using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Serialization;
using JetBrains.Annotations;
// http://referencesource.microsoft.com/#System.ComponentModel.DataAnnotations/DataAnnotations/CustomValidationAttribute.cs
// using JetBrains.Annotations;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;


namespace ConvexHullHelper
{
	public abstract class NotifyPropertyChangeBase : INotifyPropertyChanged, INotifyDataErrorInfo
	{
		// ******************************************************************
		[field: NonSerialized]
		public event PropertyChangedEventHandler PropertyChanged;

		[field: NonSerialized]
		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

		[field: NonSerialized]
		private ConcurrentDictionary<string, List<string>> _errors = new ConcurrentDictionary<string, List<string>>();

		[field: NonSerialized]
		protected object _lock = new object(); // Object to lock when a validation occured on multihtreaded validation code.

		[XmlIgnore]
		[Browsable(false)]
		public Dispatcher Dispatcher { get; set; }

		// ******************************************************************
		protected NotifyPropertyChangeBase()
		{
			Dispatcher = Application.Current.Dispatcher;
		}

		// ******************************************************************
		[NotifyPropertyChangedInvocator]
		protected void RaisePropertyChanged([CallerMemberName] String propertyName = "")
		{
			if (Dispatcher.CheckAccess())
			{
				var propertyChanged = PropertyChanged;
				propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			}
			else
			{
				Dispatcher.BeginInvoke(new Action(()=>
				{
					var propertyChanged = PropertyChanged;
					propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
				}));
			}
		}

		// ******************************************************************
		protected void RaisePropertyChanged(PropertyChangedEventArgs args)
		{
			if (Dispatcher.CheckAccess())
			{
				var propertyChanged = PropertyChanged;
				propertyChanged?.Invoke(this, args);
			}
			else
			{
				Dispatcher.BeginInvoke(new Action(() =>
				{
					var propertyChanged = PropertyChanged;
					propertyChanged?.Invoke(this, args);
				}));
			}
		}

		// ******************************************************************
		protected virtual void SetProperty<T>(ref T member, T val, [CallerMemberName] string propertyName = null)
		{
			if (object.Equals(member, val)) return;

			member = val;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		// ******************************************************************
		[NotifyPropertyChangedInvocator]
		protected void OnErrorsChanged([CallerMemberName] String propertyName = "")
		{
			if (ErrorsChanged != null)
			{
				ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
			}
		}

		// ******************************************************************
		[XmlIgnore]
		[Browsable(false)]
		public bool HasErrors
		{
			get
			{
				return _errors.Count > 0;
			}
		}

		// ******************************************************************
		public IEnumerable GetErrors(string propertyName)
		{
			if (propertyName == null)
			{
				return null;
			}

			List<string> propertyErrors = null;
			_errors.TryGetValue(propertyName, out propertyErrors);
			return propertyErrors;
		}

		// ******************************************************************
		/// <summary>
		/// Calling with null will remove any existing errors if any.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="validationResults"></param>
		public void SetErrorFor<T>(Expression<Func<T>> property, List<string> validationResults = null)
		{
			var asMember = property.Body as MemberExpression;
			if (asMember == null)
			{
				throw new ArgumentException("Unable to find property name.");
			}

			string propertyName = asMember.Member.Name;

			if (validationResults == null)
			{
				// Remove the existing errors for this property
				List<string> fakeListForRemoval;
				if (_errors.TryRemove(propertyName, out fakeListForRemoval))
				{
					OnErrorsChanged(propertyName);
				}
			}
			else
			{
				// Add or Update
				_errors[propertyName] = validationResults;
				OnErrorsChanged(propertyName);
			}
		}

		// ******************************************************************
		/// <summary>
		/// Calling with null will remove any existing errors if any.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="validationResults"></param>
		public void SetErrorFor<T>(Expression<Func<T>> property, ValidationResult validationResult = null)
		{
			var asMember = property.Body as MemberExpression;
			if (asMember == null)
			{
				throw new ArgumentException("Unable to find property name.");
			}

			string propertyName = asMember.Member.Name;

			if (validationResult == null || validationResult.ErrorMessage == null)
			{
				// Remove the existing errors for this property
				List<string> fakeListForRemoval;
				if (_errors.TryRemove(propertyName, out fakeListForRemoval))
				{
					OnErrorsChanged(propertyName);
				}
			}
			else
			{
				// Add or Update
				var errorList = new List<string>(1);
				errorList.Add(validationResult.ErrorMessage);
				_errors[propertyName] = errorList;
				OnErrorsChanged(propertyName);
			}
		}

		// ******************************************************************
		/// <summary>
		/// Calling with null will remove any existing errors if any.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="error"></param>
		public void SetErrorFor<T>(Expression<Func<T>> property, string errorMsg = null)
		{
			var asMember = property.Body as MemberExpression;
			if (asMember == null)
			{
				throw new ArgumentException("Unable to find property name.");
			}

			string propertyName = asMember.Member.Name;

			if (errorMsg == null)
			{
				// Remove the existing errors for this property
				List<string> fakeListForRemoval;
				if (_errors.TryRemove(propertyName, out fakeListForRemoval))
				{
					OnErrorsChanged(propertyName);
				}
			}
			else
			{
				// Add or Update
				var errorList = new List<string>(1);
				errorList.Add(errorMsg);
				_errors[propertyName] = errorList;
				OnErrorsChanged(propertyName);
			}
		}

		// ******************************************************************
		/// <summary>
		/// Validate only one property value
		/// </summary>
		/// <param name="value">Value to test</param>
		/// <param name="propertyName"></param>
		/// <returns>return true if no error found otherwise true</returns>
		public bool ValidateProperty(object value, [CallerMemberName] string propertyName = null)
		{
			lock (_lock)
			{
				var validationContext = new ValidationContext(this, null, null);
				validationContext.MemberName = propertyName;
				var validationResults = new List<ValidationResult>();
				Validator.TryValidateProperty(value, validationContext, validationResults);

				//clear previous errors from tested property
				var propNamesToNotify = new List<string>();
				List<string> fakeListForRemoval;
				if (_errors.TryRemove(propertyName, out fakeListForRemoval))
				{
					propNamesToNotify.Add(propertyName);
				}

				return HandleValidationResults(validationResults, propNamesToNotify);
			}
		}

		// ******************************************************************
		/// <summary>
		/// Override in order to be able to call with '()=>PropertyName' instead of a string where potential
		/// PropertyName changes error would be discovered at compile time instead of runtime.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value">Value to test</param>
		/// <param name="property"></param>
		protected bool ValidateProperty<T>(object value, Expression<Func<T>> property)
		{
			var asMember = property.Body as MemberExpression;
			if (asMember == null)
			{
				throw new ArgumentException("Unable to find property name.");
			}

			string propName = asMember.Member.Name;

			return ValidateProperty(value, propName);
		}

		// ******************************************************************
		/// <summary>
		/// Validate all properties with ValidationAttributes of this object (derived from this object).
		/// </summary>
		/// <returns>return true if no error found otherwise true</returns>
		public bool Validate()
		{
			lock (_lock)
			{
				var validationContext = new ValidationContext(this, null, null);
				var validationResults = new List<ValidationResult>();
				Validator.TryValidateObject(this, validationContext, validationResults, true);

				//clear all previous errors
				var propNamesToNotify = _errors.Keys.ToList();
				_errors.Clear();

				return HandleValidationResults(validationResults, propNamesToNotify);
			}
		}

		// ******************************************************************
		private bool HandleValidationResults(List<ValidationResult> validationResults, List<string> propNamesToNotify)
		{
			bool isValid = true;

			//Group validation results by property names
			var resultsByPropNames = from validationResult in validationResults
									 from mname in validationResult.MemberNames
									 group validationResult by mname into g
									 select g;

			//add errors to dictionary and inform  binding engine about errors
			foreach (var prop in resultsByPropNames)
			{
				var messages = prop.Select(r => r.ErrorMessage).ToList();
				if (!_errors.TryAdd(prop.Key, messages))
				{
					throw new Exception("Unable to add an item into _errors");
				}

				if (!propNamesToNotify.Contains(prop.Key))
				{
					propNamesToNotify.Add(prop.Key);
				}

				isValid = false;
			}

			propNamesToNotify.ForEach(pn => OnErrorsChanged(pn));

			return isValid;
		}

		// ******************************************************************
		protected void RaisePropertyChanged<T2>(Expression<Func<T2>> propAccess)
		{
			PropertyChangedEventHandler propertyChanged = PropertyChanged;
			if (propertyChanged != null)
			{
				var asMember = propAccess.Body as MemberExpression;
				if (asMember == null)
				{
					throw new ArgumentException("Invalid property");
				}

				// To try to debug : 'FatalExecutionEngineError'
				string propName = asMember.Member.Name;

				RaisePropertyChanged(propName);
			}
		}

		// ******************************************************************
		/// <summary>
		/// It affect the value only if it is different and notify also in the meantime.
		/// How to call: RaisePropertyChanged(ref field, value);
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="field"></param>
		/// <param name="value"></param>
		/// <param name="caller"></param>
		protected virtual void ChangeProperty<T>(ref T field, T value, [CallerMemberName] string caller = null)
		{
			if (!EqualityComparer<T>.Default.Equals(field, value))
			{
				field = value;
				var propertyChanged = PropertyChanged;
				if (propertyChanged != null)
				{
					propertyChanged(this, new PropertyChangedEventArgs(caller));
				}

			}
		}

		// ******************************************************************




	}
}






