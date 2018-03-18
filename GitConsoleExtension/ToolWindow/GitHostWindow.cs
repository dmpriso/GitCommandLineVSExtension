//------------------------------------------------------------------------------
// <copyright file="GitHostWindow.cs" company="Microsoft">
//     Copyright (c) Microsoft.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using EnvDTE80;
using Microsoft.VisualStudio.Shell.Interop;

namespace GitConsoleExtension
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("bc7b9662-5078-4385-9d4d-002d7e4fd1ea")]
    public class GitHostWindow : ToolWindowPane
    {
        DTE2 _dte2;
        EnvDTE.Events _events;
        EnvDTE.SolutionEvents _events2;
        EnvDTE.WindowEvents _events3;

        GitHostWindowControl _githost;

        /// <summary>
        /// Initializes a new instance of the <see cref="GitHostWindow"/> class.
        /// </summary>
        public GitHostWindow() : base(null)
        {
            // save references in order to make event registrations are not GCed
            _dte2 = (DTE2)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SDTE));
            _events = _dte2.Events;
            _events2 = _events.SolutionEvents;
            _events2.Opened += SolutionEvents_Opened;
            _events3 = _events.WindowEvents;
            _events3.WindowActivated += _events3_WindowActivated;

            this.Caption = "Git Integrated Window";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            this.Content = _githost = new GitConsoleExtension.GitHostWindowControl();
        }

        private void _events3_WindowActivated(EnvDTE.Window GotFocus, EnvDTE.Window LostFocus)
        {
            System.Diagnostics.Debug.WriteLine(GotFocus.Caption);
            if (GotFocus.Caption == this.Caption)
                _githost.setFocus();
        }

        private void SolutionEvents_Opened()
        {
            _githost.reloadHost();
        }
    }
}
