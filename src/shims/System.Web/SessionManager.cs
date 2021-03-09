// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace System.Web
{
    internal class SessionManager : ISessionManager
    {
        private readonly Dictionary<string, LinkedListNode<SessionEntry>> _cache;
        private readonly LinkedList<SessionEntry> _list;

        public SessionManager()
        {
            _cache = new Dictionary<string, LinkedListNode<SessionEntry>>();
            _list = new LinkedList<SessionEntry>();
        }

        public HttpSessionStateBase GetState(string sessionId)
        {
            lock (_cache)
            {
                if (_cache.TryGetValue(sessionId, out var sessionNode))
                {
                    var current = sessionNode.Value;

                    _list.Remove(sessionNode);
                    return Add(current.Session);
                }

                return Add(new HttpSessionStateBase(sessionId));
            }
        }

        private HttpSessionStateBase Add(HttpSessionStateBase session)
        {
            var entry = new SessionEntry(DateTimeOffset.UtcNow, session);
            var node = _list.AddLast(entry);
            _cache[session.Id] = node;
            return session;
        }

        public int RemoveStaleSessions(TimeSpan age)
        {
            lock (_cache)
            {
                var count = 0;
                var current = _list.First;
                var cutoff = DateTimeOffset.UtcNow.Subtract(age);

                while (current is not null && current.Value.LastAccessed < cutoff)
                {
                    var node = current;
                    current = node.Next;
                    _list.Remove(node);
                    count++;
                }

                return count;
            }
        }

        private record SessionEntry(DateTimeOffset LastAccessed, HttpSessionStateBase Session);
    }
}
