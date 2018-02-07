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
using System.Collections.Generic;
using System.Linq;

namespace XYZ.Data.SQLite
{
    public class SQLiteDataSource : XYZ.Data.IDataSource {
        #region Fields
        private SQLite4Unity3d.SQLiteConnection _connection;
        private SQLite4Unity3d.SQLiteConnectionString _connectionString;
        #endregion Fields
        #region Properties
        public SQLite4Unity3d.SQLiteConnection Connection {
            get {
                if (this._connection == null) {
                    this._connection = new SQLite4Unity3d.SQLiteConnection(this.DatabasePath, this.SaveTimeAsTicks);
                }
                return this._connection;
            }
        }
        public String ConnectionString { get { return this._connectionString.ConnectionString; } set { } }
        public String DatabasePath { get; set; } 
        public Boolean SaveTimeAsTicks { get; set; }
        #endregion Properties
        #region .tor
        public SQLiteDataSource(String DatabasePath, Boolean SaveTimeAsTicks = false) {
            this.DatabasePath = DatabasePath;
            this.SaveTimeAsTicks = SaveTimeAsTicks;
        }

        public SQLiteDataSource() { this.SaveTimeAsTicks = false; }
        #endregion .tor
        #region Methods
        #region IDispose Methods
        public void Dispose() {
            this._connection?.Close();
            this._connection?.Dispose();
        }
        #endregion IDispose Methods
        #region IDataSource Methods
        public void BeginTransaction() {
            this.Connection.BeginTransaction();
        }


        public void EndTransaction() {
            this.Connection.Commit();
        }

        public List<TModel> Query<TModel>(String Query, params object[] Arguments) where TModel : new() {
            return this.Connection.Query<TModel>(Query, Arguments);
        }

        public int Save<TModel>(TModel EntryToSave) where TModel : new() {
            //this.Connection.Delete(EntryToSave);

            return this.Connection.InsertOrReplace(EntryToSave);
           
        }

        public TReturnType Scalar<TReturnType>(String Query, params Object[] Arguments) {
            return this.Connection.ExecuteScalar<TReturnType>(Query);
        }

        public List<TModel> Table<TModel>() where TModel : new() {
            return this.Connection.Table<TModel>().Distinct().ToList();
        }

        public void CreateTable<TModel>() where TModel : new() {
            this.Connection.CreateTable<TModel>();
        }

        public void Delete<TModel>(TModel EntryToDelete) where TModel : new() {
            this.Connection.Delete(EntryToDelete);
        }

        public void DeleteAll<TModel>() where TModel : new() {
            this.Connection.DeleteAll<TModel>();
        }

        public void NonQuery(String Query, params Object[] Arguments) {
            this.Connection.Execute(Query, Arguments);
        }

        public void Open() {
            
        }

        public void Close() {
            this.Dispose();
        }
        #endregion IDataSource Methods
        #endregion Methods
    }
}
