#region CopyRight (c) 2018, Cao Smith: Jagged/Verge
/*  
 * XYZ is a .NET 4.6 abstraction and implementation Framework designed for Unity and other .NET/Mono platforms
 *
 * XYZ is a toolkit designed to ease the burden of to-the-metal system functionality and to strongly separate business concerns.
 * Implemented as an abstraction library, XYZ provides full implementation tools for:
 *
 * - creating strongly bound, yet firmly separated concerns in any tier
 * - Creating View Logic for UI
 * - Creating Robust Models
 * - Tightly Integrated Domain code for acting with or without a View alongside Models
 * - Sensible wrapper layers to facilitate quickly swapping out backend logic without nessicitating rewriting consumption code
 * 
 * In plain terms:
 * XYZ allows you to create highly maintainable, cleanly separated code. 
 * XYZ allows you to implement a Model/View/Glue pattern in platforms such as UNITY, MONO & WinForms.
 *
 *
 * http://www.jaggedverge.com
 *
 *  This library is dual licensed under the BSD license for Free and Open Source projects. Any utilizing works that in whole
 * or part utilize this work in Source or Binary form and apply charges to any aspect of the utilizing works (such as 
 * commercial distribution, service fees or distribution fees), must negotiate with the copyright owner a fair and amicable
 * license. This is inclusive of pure binary linking, inclusion or other common FOSS accomodations and work arounds.
 * 
 * The following license applies to any individual, organization or company who is creating software for said entity and not
 * applying any sort of charge to it's distribution to or consumption by end users. This means any commercial enterprise or academic 
 * institute, etc. is unrestricted in making internal tools for their organization, provided no charges as outlined in the above
 * paragraph are applied.  
 * 
 * Copyright (c) 2015-2020, Cao Smith, Jagged/Verge
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 * 1. Redistributions of source code must retain the above copyright notice, this
 *    list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright notice,
 *    this list of conditions and the following disclaimer in the documentation
 *   and/or other materials provided with the distribution.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
 * ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 *  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
#endregion CopyRight (c) 2018, Cao Smith: Jagged/Verge
using System;

namespace XYZ.ComponentModel
{
    public interface IModel : INoticeable, ICloneable, IDisposable
    {
    }
    public abstract class XYZModel :  XYZNotifyPropertyChanged, IModel
    {
        #region Properties
        public virtual int? ID { get; set; }
        #endregion Properties
        #region .tor
        public XYZModel() { }
        #endregion .tor
        #region Methods
        #region Override Methods
        /// <summary>
        /// Compares an object to the current object
        /// </summary>
        /// <param name="Comparee">Object to test</param>
        /// <returns>True if equal</returns>
         public override Boolean Equals(Object Comparee) => ((Comparee is XYZModel) && (this == (Comparee as XYZModel)));
        
        /// <summary>
        /// Returns the hashcode
        /// </summary>
        /// <returns>Hashcode of ID</returns>
        public override int GetHashCode() => this.ID.GetHashCode();

        #endregion Override Methods
        #region ICloneable Methods
        /// <summary>
        /// Makes a value copy of current object
        /// </summary>
        /// <returns>A duplicate of the current object without reference (by value)</returns>
        public virtual Object Clone() =>  null;
        #endregion ICloneable Methods
        #region IDispose Methods
        public virtual void Dispose() { }
        #endregion IDispose Methods
        #region Copying Methods
        public virtual void CopyTo(Object Dst) { XYZModels.Copy(this, (Dst as XYZModel)); }
        public virtual void CopyFrom(Object Src) { XYZModels.Copy((Src as XYZModel), this); }
        #endregion Copying Methods
        #region Identity Methods
        public virtual Boolean Is(Object Comparee) => XYZModels.Is(this, (Comparee as XYZModel));
        #endregion Identity Methods
        #region Event Handler Methods
        protected virtual void OnCollectionChanged(Object Sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs Arghs) { }
        #endregion Event Handler Methods
        #region Reference Handler Methods
        private Boolean SetReferencedModelProperty(Object Target, String ReferencedPropertyToSetName, Object Value, Boolean DoUnset = false) {
            Boolean result = false;
            System.Reflection.PropertyInfo propertyToSet = Target.GetType().GetProperty(ReferencedPropertyToSetName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (propertyToSet != null) {
                if (propertyToSet.CanWrite) {
                    if (propertyToSet.PropertyType == Value.GetType()) {
                        Object currentValue = propertyToSet.GetValue(Target);
                        Object newValue = Value;
                        if (DoUnset) {
                            if (currentValue == Value) {
                                newValue = null;
                            }
                        }
                        if (currentValue != newValue) {
                            propertyToSet.SetValue(Target, Value, null);
                        }
                        result = true;
                    }
                }
            }

            return result;
        }

        protected void DoChangeEvent<TModel>(System.Collections.Specialized.NotifyCollectionChangedEventArgs Arghs, String PropertyToReferenceName = null) {
            if ((Arghs.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add) || (Arghs.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)) {
                foreach (TModel model in Arghs.NewItems) {
                    this.SetReferencedModelProperty(model, PropertyToReferenceName, this);
                }
            }

            if ((Arghs.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove) || (Arghs.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)) {
                foreach (TModel model in Arghs.OldItems) {
                    this.SetReferencedModelProperty(model, PropertyToReferenceName, this, true);
                }
            }
        }

        protected void SetCollectionReferenceFromProperty<TModel>(ref System.Collections.ObjectModel.ObservableCollection<TModel> OriginalList,
            System.Collections.ObjectModel.ObservableCollection<TModel> NewList, String PropertyToSetName = null, 
            [System.Runtime.CompilerServices.CallerMemberName] String CallerMemberName = null) where TModel : XYZModel {

            if (OriginalList == NewList) {
                return;
            }

            Boolean doProcessModels = (!String.IsNullOrWhiteSpace(PropertyToSetName));
            if (OriginalList != null) {
                OriginalList.CollectionChanged -= this.OnCollectionChanged;
                if (doProcessModels) {
                    foreach (TModel model in OriginalList) {
                        this.SetReferencedModelProperty(model, PropertyToSetName, this, true);
                    }
                }
            }
            if (NewList != null) {
                NewList.CollectionChanged += this.OnCollectionChanged;
                if (doProcessModels) {
                    foreach (TModel model in NewList) {
                        this.SetReferencedModelProperty(model, PropertyToSetName, this);
                    }
                }
            }
            this.SetProperty(ref OriginalList, NewList, CallerMemberName: CallerMemberName);
        }

        protected void BackSetTargetListFromProperty<TReferenceModel, TListModel>(ref TReferenceModel TargetProperty, 
                                                                TReferenceModel Value, 
                                                                System.Collections.Generic.IList<TListModel> TargetPropertyList, 
                                                                System.Collections.Generic.IList<TListModel> ValueList, 
                                                                [System.Runtime.CompilerServices.CallerMemberName] String CallerMemberName = null) 
            where TReferenceModel : XYZModel, new() 
            where TListModel : XYZModel, new(){

            if (TargetProperty != Value) {
                TListModel self = (this as TListModel);
                TReferenceModel oldValue = TargetProperty;
                this.SetProperty(ref TargetProperty, Value, CallerMemberName: CallerMemberName);
                if (oldValue != null) {
                    TargetPropertyList?.Remove(self);
                }
                if (!(ValueList?.Contains(self)).GetValueOrDefault(true)) {
                    ValueList.Add(self);
                }
            }
        }
        #endregion Reference Handler Methods
        #endregion Methods
    }
}
