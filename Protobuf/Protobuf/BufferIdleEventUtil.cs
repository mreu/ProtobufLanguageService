// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BufferIdleEventUtil.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MichaelReukauff.Protobuf
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Threading;

    using Microsoft.VisualStudio.Text;

    /// <summary>
    /// Handy reusable utility to listen for change events on the associated buffer, but
    /// only pass these along to a set of listeners when the user stops typing for a half second.
    /// </summary>
    internal static class BufferIdleEventUtil
    {
        /// <summary>
        /// The buffer listeners key.
        /// </summary>
        private static readonly object BufferListenersKey = new object();

        /// <summary>
        /// The buffer timer key.
        /// </summary>
        private static readonly object BufferTimerKey = new object();

        #region Public Interface
        /// <summary>
        /// The add buffer idle event listener.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="handler">
        /// The handler.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool AddBufferIdleEventListener(ITextBuffer buffer, EventHandler handler)
        {
            HashSet<EventHandler> listenersForBuffer;

            if (!TryGetBufferListeners(buffer, out listenersForBuffer))
            {
                listenersForBuffer = ConnectToBuffer(buffer);
            }

            if (listenersForBuffer.Contains(handler))
            {
                return false;
            }

            listenersForBuffer.Add(handler);

            return true;
        }

        /// <summary>
        /// The remove buffer idle event listener.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="handler">
        /// The handler.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool RemoveBufferIdleEventListener(ITextBuffer buffer, EventHandler handler)
        {
            HashSet<EventHandler> listenersForBuffer;

            if (!TryGetBufferListeners(buffer, out listenersForBuffer))
            {
                return false;
            }

            if (!listenersForBuffer.Contains(handler))
            {
                return false;
            }

            listenersForBuffer.Remove(handler);

            if (listenersForBuffer.Count == 0)
            {
                DisconnectFromBuffer(buffer);
            }

            return true;
        }
        #endregion Public Interface

        #region Helpers
        /// <summary>
        /// The try get buffer listeners.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="listeners">
        /// The listeners.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool TryGetBufferListeners(ITextBuffer buffer, out HashSet<EventHandler> listeners)
        {
            return buffer.Properties.TryGetProperty(BufferListenersKey, out listeners);
        }

        /// <summary>
        /// The clear buffer listeners.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        private static void ClearBufferListeners(ITextBuffer buffer)
        {
            buffer.Properties.RemoveProperty(BufferListenersKey);
        }

        /// <summary>
        /// The try get buffer timer.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="timer">
        /// The timer.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool TryGetBufferTimer(ITextBuffer buffer, out DispatcherTimer timer)
        {
            return buffer.Properties.TryGetProperty(BufferTimerKey, out timer);
        }

        /// <summary>
        /// The clear buffer timer.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        private static void ClearBufferTimer(ITextBuffer buffer)
        {
            DispatcherTimer timer;

            if (TryGetBufferTimer(buffer, out timer))
            {
                if (timer != null)
                {
                    timer.Stop();
                }

                buffer.Properties.RemoveProperty(BufferTimerKey);
            }
        }

        /// <summary>
        /// The disconnect from buffer.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        private static void DisconnectFromBuffer(ITextBuffer buffer)
        {
            buffer.Changed -= BufferChanged;

            ClearBufferListeners(buffer);
            ClearBufferTimer(buffer);

            buffer.Properties.RemoveProperty(BufferListenersKey);
        }

        /// <summary>
        /// The connect to buffer.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <returns>
        /// The <see cref="HashSet{EventHandler}"/>.
        /// </returns>
        private static HashSet<EventHandler> ConnectToBuffer(ITextBuffer buffer)
        {
            buffer.Changed += BufferChanged;

            RestartTimerForBuffer(buffer);

            var listenersForBuffer = new HashSet<EventHandler>();
            buffer.Properties[BufferListenersKey] = listenersForBuffer;

            return listenersForBuffer;
        }

        /// <summary>
        /// The restart timer for buffer.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        private static void RestartTimerForBuffer(ITextBuffer buffer)
        {
            DispatcherTimer timer;

            if (TryGetBufferTimer(buffer, out timer))
            {
                timer.Stop();
            }
            else
            {
                timer = new DispatcherTimer(DispatcherPriority.ApplicationIdle)
                {
                    Interval = TimeSpan.FromMilliseconds(500)
                };

                timer.Tick += (s, e) =>
                {
                    ClearBufferTimer(buffer);

                    HashSet<EventHandler> handlers;
                    if (TryGetBufferListeners(buffer, out handlers))
                    {
                        foreach (var handler in handlers)
                        {
                            handler(buffer, new EventArgs());
                        }
                    }
                };

                buffer.Properties[BufferTimerKey] = timer;
            }

            timer.Start();
        }

        /// <summary>
        /// The buffer changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void BufferChanged(object sender, TextContentChangedEventArgs e)
        {
            var buffer = sender as ITextBuffer;

            if (buffer == null)
            {
                return;
            }

            RestartTimerForBuffer(buffer);
        }
        #endregion Helpers
    }
}