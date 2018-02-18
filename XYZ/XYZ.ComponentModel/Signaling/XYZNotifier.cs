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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
/*
 * This file is part of the BedRock library.
 * BedRock is a loosly coupled framework and software foundation API.
 * 
 * Copyright (c) 2016 Cao & Jagged\Verge
 * 
 * OPEN-SOURCE PROJECT LICENSING
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial EditorModelions of the Software.
 *
 *  FREE CLOSED SOURCE:
 *  The above applies as pertaining to no monetary charges. The cost for this license is furnishing modifications made to this library be provided 
 *  directly to the copyright holders and an honourary license to the project it was used for in trade for the work.
 *  
 *  COMMERCIAL CLOSE or OPEN SOURCE:
 * The copyright holders require the licensee to pay a percentage of residuals/fees/retainers commiserate with the EditorModelion of your product this
 * library comprises.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR 
 * OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 * DEALINGS IN THE SOFTWARE.
 *
 * 
 */   
    namespace XYZ.ComponentModel {

    #region Notice Interfaces
    public interface INoticeable  {
        #region Properties
        int? ID { get; }
        #endregion Properties
    }

    public interface INoticeReceiver  {
        void ReceiveNotice(NoticesArg notices);
    }

    #endregion Notice Interfaces
    #region Notice Enums
    public enum NoticeTypeValues {
        Change,
        Add,
        Remove,
        Complete,
        Started,
        Error
    }
    #endregion Notice Enums
    #region Notice Arguments
    public class NoticesArg  {
        #region Properties
        public NoticeTypeValues NoticeType { get; set; }
        public Dictionary<Type, Dictionary<long, INoticeable>> Notices = new Dictionary<Type, Dictionary<long, INoticeable>>();
        #endregion Properties

        #region .ctor
        public NoticesArg(NoticeTypeValues NoticeType, INoticeable Notice) {
            this.NoticeType = NoticeType;
            Add(Notice);
        }

        public NoticesArg(NoticeTypeValues NoticeType) {
            this.NoticeType = NoticeType;
        }

        public NoticesArg(NoticeTypeValues NoticeType, IEnumerable<INoticeable> Notices) {
            this.NoticeType = NoticeType;
            foreach (INoticeable notice in Notices) {
                Add(notice);
            }
        }

        #endregion .ctor

        #region Functions
        #region Functions - Check
        public virtual Boolean Has(Type type) {
            
                
            return ((Notices.ContainsKey(type)) && (type is INoticeable));
        }

        public virtual Boolean Contains(INoticeable Notice) {
            Boolean result = false;
            if (Notices.ContainsKey(Notice.GetType())) {
                result = Notices[Notice.GetType()].ContainsKey(Notice.ID.GetValueOrDefault());
            }
            return result;
        }
        #endregion Functions - Check
        #region Functions - Retrieve
        public virtual IEnumerable<TNoticeType> GetAll<TNoticeType>(TNoticeType NoticeType) where TNoticeType : INoticeable {
            IEnumerable<TNoticeType> results = (IEnumerable<TNoticeType>)new List<TNoticeType>();
            if (Notices.ContainsKey(NoticeType.GetType())) {
                results = Notices[NoticeType.GetType()].Values.Select(obj => ((TNoticeType)obj));
            }
            return results;
        }
        #endregion Functions - Retrieve



        public virtual void Add(INoticeable Notice)  {
            if (!Notices.ContainsKey(Notice.GetType())) {
                Notices[Notice.GetType()] = new Dictionary<long, INoticeable>();
            }
            Notices[Notice.GetType()][Notice.ID.GetValueOrDefault()] = Notice;
        }
        #endregion Functions
    }
    #endregion Notice Arguments

    public static class NoticeBroadcaster {

        private static HashSet<INoticeReceiver> _universalReceivers  = new HashSet<INoticeReceiver>() ;
        private static Dictionary<Type, HashSet<INoticeReceiver>> _receivers = new Dictionary<Type, HashSet<INoticeReceiver>>();

        public static  void AddReceiver(INoticeReceiver Receiver) {
            _universalReceivers.Add(Receiver);
        }

        public  static void AddReceiver(INoticeReceiver Receiver, INoticeable Noticeable) {
            if (!_receivers.ContainsKey(Noticeable.GetType())) {
                _receivers[Noticeable.GetType()] = new HashSet<INoticeReceiver>();
            }
            _receivers[Noticeable.GetType()].Add(Receiver);
        }

        public static void RemoveReceipt(INoticeable Noticeable, INoticeReceiver Receiver) {
            if (_receivers.ContainsKey(Noticeable.GetType())) {
                HashSet<INoticeReceiver> receivers = _receivers[Noticeable.GetType()];
                if (receivers.Contains(Receiver)) {
                    receivers.Remove(Receiver);
                }
            }
        }

        public static void RemoveReceipt(INoticeReceiver Receiver) {
            if (_universalReceivers.Contains(Receiver)) {
                _universalReceivers.Remove(Receiver);
            }

            foreach (HashSet<INoticeReceiver> registrants in _receivers.Values) {
                if (registrants.Contains(Receiver)) {
                    registrants.Remove(Receiver);
                }
            }
        }
        public static void Broadcast(NoticesArg Notices)
        {
            System.Threading.Thread outBound = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(NoticeBroadcaster.Broadcast)) ;
            outBound.IsBackground = true;
            outBound.Start(Notices);
        }

        private static void Broadcast(object Args) {
            NoticesArg notices = (Args as NoticesArg);
            HashSet<INoticeReceiver> receivers = new HashSet<INoticeReceiver>();
            if (notices.Notices.Count() == 0) {
                return;
            }


            foreach (INoticeReceiver receiver in _universalReceivers) {
                receivers.Add(receiver);
            }

            foreach (Type noticeable in notices.Notices.Keys) {
                if (_receivers.ContainsKey(noticeable)) {
                    foreach (INoticeReceiver registrant in _receivers[noticeable]) {
                        receivers.Add(registrant);
                    }
                }
            }

            foreach (INoticeReceiver receiver in receivers) {
                    DoNotify(receiver, notices);
            }
        }

        private static void DoNotify(INoticeReceiver Recipient, NoticesArg Notices) {
            Recipient.ReceiveNotice(Notices);
        }
    }
}
