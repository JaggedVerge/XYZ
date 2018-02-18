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
 * license fee. This is inclusive of pure binary linking, inclusion or other common FOSS accomodations and work arounds.
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
namespace XYZ.ComponentModel {
    public abstract class SingleListEditableViewModel<TListBox, TListBoxItemViewModel, TEditModel, TModel> : SingleEditViewModel<TEditModel, TModel>
        where TListBox : ListBoxViewModel<TListBoxItemViewModel, TModel>, new()
        where TListBoxItemViewModel : ListBoxItemViewModel<TModel>
        where TEditModel : EditModel<TModel>, new()
        where TModel : XYZModel, new() {
        #region Properties
        public TListBox ListBox { get; }
        public  TListBoxItemViewModel ViewModel { get; set; }
        #endregion Properties
        #region .tor
        public SingleListEditableViewModel(TModel Model, TListBox ListBox =null) : base(Model){
            if (ListBox == null) {
                this.ListBox = new TListBox();
            } else {
                this.ListBox = ListBox;
            }
            this.ListBox.ViewModelAdded += OnModelAdded;
            this.ListBox.ViewModelRemoved += OnModelRemoved;
        }
        public SingleListEditableViewModel() : base() {
            this.ListBox = new TListBox();
            this.ListBox.ViewModelAdded += OnModelAdded;
            this.ListBox.ViewModelRemoved += OnModelRemoved;
        }
        #endregion .tor
        #region Methods
        #region SingleEditViewModel Overrides
        public override void Save() {
            if (this._editModel != null)  {
                base.Save();
                if (this.Model.ID == null) {
                    this.ListBox.Add(this.ViewModel);
                } else {
                    this.Save(this._editModel.Model);
                }
            }
        }

        public override void Cancel() {
            base.Cancel();
            this._editModel = null;
            this.ViewModel = null;
        }
        #endregion SingleEditViewModel Overrides
        #region Event Handler Methods
        protected void OnModelAdded(System.Object Sender, System.EventArgs Args) {
            TListBoxItemViewModel viewModel = (TListBoxItemViewModel)Sender;
            if (viewModel._model.ID == null) {
                this.Save(viewModel._model);
            }
        }
        protected void OnModelRemoved(System.Object Sender, System.EventArgs Args) {
            TListBoxItemViewModel viewModel = (TListBoxItemViewModel)Sender;
            this.Delete(viewModel._model);
        }
        protected void OnModelChanged(System.Object Sender, System.EventArgs Args) {
            this.Save();
        }
        #endregion Event Handler Methods
        public void Edit(TListBoxItemViewModel ViewModel) {
            this.SetModel(ViewModel._model);
        }
        protected abstract  TListBoxItemViewModel CreateNewItemViewModel();
        public virtual void New() {
            this.ViewModel = CreateNewItemViewModel();
            this.SetModel(this.ViewModel._model);
        }
        protected abstract void Save(TModel Model);
        protected abstract void Delete(TModel Model);
        #endregion Methods
    }
}
