//// Decompiled with JetBrains decompiler
//// Type: JetBrains.Annotations.NotifyPropertyChangedInvocatorAttribute
//// Assembly: JetBrains.Annotations, Version=8.2.1000.4556, Culture=neutral, PublicKeyToken=1010a0d8d6380325
//// MVID: A108B71F-2A6F-4010-8B21-23B2FE11BAD3
//// Assembly location: C:\Program Files (x86)\JetBrains\ReSharper\v8.2\Bin\JetBrains.Annotations.dll

//using System;

//namespace ConvexHullWorkbench
//{
//	/// <summary>
//	/// Indicates that the method is contained in a type that implements
//	///              <see cref="T:System.ComponentModel.INotifyPropertyChanged"/> interface
//	///              and this method is used to notify that some property value changed
//	/// 
//	/// </summary>
//	/// 
//	/// <remarks>
//	/// The method should be non-static and conform to one of the supported signatures:
//	/// 
//	/// <list>
//	/// 
//	/// <item>
//	/// <c>NotifyChanged(string)</c>
//	/// </item>
//	/// 
//	/// <item>
//	/// <c>NotifyChanged(params string[])</c>
//	/// </item>
//	/// 
//	/// <item>
//	/// <c>NotifyChanged{T}(Expression{Func{T}})</c>
//	/// </item>
//	/// 
//	/// <item>
//	/// <c>NotifyChanged{T,U}(Expression{Func{T,U}})</c>
//	/// </item>
//	/// 
//	/// <item>
//	/// <c>SetProperty{T}(ref T, T, string)</c>
//	/// </item>
//	/// 
//	/// </list>
//	/// 
//	/// </remarks>
//	/// 
//	/// <example>
//	/// 
//	/// <code>
//	/// public class Foo : INotifyPropertyChanged {
//	///                public event PropertyChangedEventHandler PropertyChanged;
//	///                [NotifyPropertyChangedInvocator]
//	///                protected virtual void NotifyChanged(string propertyName) { ... }
//	/// 
//	///                private string _name;
//	///                public string Name {
//	///                  get { return _name; }
//	///                  set { _name = value; NotifyChanged("LastName"); /* Warning */ }
//	///                }
//	///              }
//	/// 
//	/// </code>
//	/// 
//	///              Examples of generated notifications:
//	/// 
//	/// <list>
//	/// 
//	/// <item>
//	/// <c>NotifyChanged("Property")</c>
//	/// </item>
//	/// 
//	/// <item>
//	/// <c>NotifyChanged(() =&gt; Property)</c>
//	/// </item>
//	/// 
//	/// <item>
//	/// <c>NotifyChanged((VM x) =&gt; x.Property)</c>
//	/// </item>
//	/// 
//	/// <item>
//	/// <c>SetProperty(ref myField, value, "Property")</c>
//	/// </item>
//	/// 
//	/// </list>
//	/// 
//	/// </example>
//	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
//	public sealed class NotifyPropertyChangedInvocatorAttribute : Attribute
//	{
//		public string ParameterName { get; private set; }

//		public NotifyPropertyChangedInvocatorAttribute()
//		{
//		}

//		public NotifyPropertyChangedInvocatorAttribute(string parameterName)
//		{
//			this.ParameterName = parameterName;
//		}
//	}
//}
