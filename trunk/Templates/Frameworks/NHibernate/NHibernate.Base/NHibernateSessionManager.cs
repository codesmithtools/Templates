using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NHibernate;
using NHibernate.Cfg;

namespace NHibernate.Base
{
    /// <summary>
    /// A Singleton that creates and persits a single SessionFactory for the to program to access globally.
    /// This uses the .Net CallContext to store a session for each thread.
    /// 
    /// This is heavely based on 'NHibernate Best Practices with ASP.NET' By Billy McCafferty.
    /// http://www.codeproject.com/KB/architecture/NHibernateBestPractices.aspx
    /// </summary>
    public class NHibernateSessionManager : IDisposable
    {
        #region Static Content

        private static NHibernateSessionManager _nHibernateSessionManager = new NHibernateSessionManager();
        public static NHibernateSessionManager Instance
        {
            get { return _nHibernateSessionManager; }
        }

        #endregion

        #region Declarations

        private ISessionFactory _sessionFactory = null;
        private const string _sessionContextKey = "NHibernateSession-ContextKey";

        #endregion

        #region Constructors & Finalizers

        /// <summary>
        /// This will load the NHibernate settings from the App.config.
        /// Note: This can/should be expanded to support multiple databases.
        /// </summary>
        private NHibernateSessionManager()
        {
            _sessionFactory = new NHibernate.Cfg.Configuration().Configure().BuildSessionFactory();
        }
        ~NHibernateSessionManager()
        {
            Dispose(true);
        }

        #endregion

        #region IDisposable

        private bool _isDisposed = false;
        public void Dispose()
        {
            Dispose(false);
        }
        private void Dispose(bool finalizing)
        {
            if (!_isDisposed)
            {
                // Close SessionFactory
                _sessionFactory.Close();
                _sessionFactory.Dispose();

                // Flag as disposed.
                _isDisposed = true;
                if (!finalizing)
                    GC.SuppressFinalize(this);
            }
        }

        #endregion

        #region Methods

        public INHibernateSession GetNewSession()
        {
            NHibernateSession session;

            lock (_sessionFactory)
            {
                session = new NHibernateSession(_sessionFactory.OpenSession());
            }

            return session;
        }
        public INHibernateSession GetContextSession()
        {
            INHibernateSession session = ContextSession;

            // If the thread does not yet have a session, create one.
            if (session == null)
            {
                session = GetNewSession();

                // Save to CallContext.
                ContextSession = session;
            }

            return session;
        }

        #endregion

        #region Properties

        private INHibernateSession ContextSession
        {
            get
            {
                if (IsWebContext)
                    return (NHibernateSession)System.Web.HttpContext.Current.Items[_sessionContextKey];
                else
                    return (NHibernateSession)System.Runtime.Remoting.Messaging.CallContext.GetData(_sessionContextKey);
            }
            set
            {
                if (IsWebContext)
                    System.Web.HttpContext.Current.Items[_sessionContextKey] = value;
                else
                    System.Runtime.Remoting.Messaging.CallContext.SetData(_sessionContextKey, value);
            }
        }
        private bool IsWebContext
        {
            get { return (System.Web.HttpContext.Current != null); }
        }

        #endregion
    }
}
