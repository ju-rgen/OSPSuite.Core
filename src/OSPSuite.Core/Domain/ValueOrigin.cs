﻿using System;
using System.Linq;
using OSPSuite.Assets;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.Core.Domain
{
   public class ValueOrigin : IComparable<ValueOrigin>, IComparable
   {
      /// <summary>
      ///    Key computed based on properties to assess if values origin are equals
      /// </summary>
      private string _key;

      private ValueOriginSource _source = ValueOriginSources.Undefined;
      private ValueOriginDeterminationMethod _method = ValueOriginDeterminationMethods.Undefined;
      private string _description;

      /// <summary>
      ///    Id of the given ValueOrigin. This is the database entry for predefined parameters and is set to null if no entry is
      ///    available
      ///    <remarks>Id is not part of equakity check</remarks>
      /// </summary>
      public int? Id { get; set; }

      /// <summary>
      ///    Indicates if a parameter value is a default value (e.g. coming from the PK-Sim database) or if the value was entered
      ///    or modified by the user. Default value is <c>false</c>
      ///    <remarks>Default is not part of equality check </remarks>
      /// </summary>
      public bool Default { get; set; }

      /// <summary>
      ///    Source of the value
      /// </summary>
      public ValueOriginSource Source
      {
         get => _source;
         set
         {
            _source = value;
            resetKey();
         }
      }

      /// <summary>
      ///    Determination method of the value
      /// </summary>
      public ValueOriginDeterminationMethod Method
      {
         get => _method;
         set
         {
            _method = value;
            resetKey();
         }
      }

      /// <summary>
      ///    Optional description explaining the quantity value
      /// </summary>
      public string Description
      {
         get => _description;
         set
         {
            _description = value;
            resetKey();
         }
      }

      private void resetKey()
      {
         _key = null;
      }

      public ValueOrigin Clone()
      {
         var clone = new ValueOrigin();
         clone.UpdateFrom(this, updateId: true);
         return clone;
      }

      public void UpdateFrom(ValueOrigin valueOrigin, bool updateId = false)
      {
         if (valueOrigin == null)
            return;

         //Id is only updated when creating value origin from database. Otherwise it should never change.
         if (updateId)
            Id = valueOrigin.Id;

         Source = valueOrigin.Source;
         Method = valueOrigin.Method;
         Description = valueOrigin.Description;
         Default = valueOrigin.Default;
      }

      public string Display => defaultDisplay(this);

      public int CompareTo(ValueOrigin other)
      {
         return string.Compare(key, other.key, StringComparison.Ordinal);
      }

      public int CompareTo(object obj)
      {
         return CompareTo(obj.DowncastTo<ValueOrigin>());
      }

      public override bool Equals(object obj)
      {
         return Equals(obj as ValueOrigin);
      }

      public bool Equals(ValueOrigin other)
      {
         if (ReferenceEquals(null, other)) return false;
         if (ReferenceEquals(this, other)) return true;
         return Equals(other.key, key);
      }

      public override int GetHashCode()
      {
         return key.GetHashCode();
      }

      public override string ToString() => Display;

      //TEMP to ensure that we can test the best display text from the app
      public static Func<ValueOrigin, string> DisplayFunc = defaultDisplay;

      private static string defaultDisplay(ValueOrigin valueOrigin)
      {
         if (isUndefined(valueOrigin))
            return Captions.ValueOrigins.Undefined;

         return new[]
         {
            valueOrigin.Source.Display, valueOrigin.Method.Display, valueOrigin.Description
         }.Where(x => !string.IsNullOrWhiteSpace(x)).ToString("-");
      }

      private static bool isUndefined(ValueOrigin valueOrigin)
      {
         if (valueOrigin.Source == null || valueOrigin.Method == null)
            return true;

         return valueOrigin.Source == ValueOriginSources.Undefined &&
                valueOrigin.Method == ValueOriginDeterminationMethods.Undefined &&
                string.IsNullOrEmpty(valueOrigin.Description);
      }

      private string key
      {
         get
         {
            if (!string.IsNullOrEmpty(_key))
               return _key;

            if (isUndefined(this))
               _key = string.Empty;

            _key = new[]
            {
               Source.Id.ToString(), Method.Id.ToString(), Description
            }.Where(x => !string.IsNullOrWhiteSpace(x)).ToString("-");

            return _key;
         }
      }
   }
}