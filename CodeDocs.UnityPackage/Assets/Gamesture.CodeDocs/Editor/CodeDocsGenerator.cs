﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace Gamesture.CodeDocs
{
    /** \ingroup editor-tools */
    
    /// <summary>
    /// Generates documentation for all classes
    /// </summary>
    public static class CodeDocsGenerator
    {
        private static readonly Queue<Action> _actions;

        private static Action<string> _updateTextAction;
        private static Action _closePopup;
        private static Thread _mainThread;
        private static bool _shouldClosePopup;

        static CodeDocsGenerator()
        {
            _actions = new Queue<Action>();
            _updateTextAction = null;
        }

        private static void Update()
        {
            if (_mainThread == null)
            {
                _mainThread = Thread.CurrentThread;
            }

            lock (_actions)
            {
                while (_actions.Count > 0)
                {
                    Action action = _actions.Dequeue();
                    action?.Invoke();
                }
            }

            if (_shouldClosePopup)
            {
                _shouldClosePopup = false;
                _closePopup?.Invoke();
                EditorApplication.update -= Update;
            }
        }

        private static Process StartCommand(string command, params string[] args)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo(command, string.Join(" ", args))
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                ErrorDialog = true,
            };


            Process process = new Process
            {
                StartInfo = processInfo, 
                EnableRaisingEvents = true
            };
            
            process.ErrorDataReceived += ErrorReceivedEventHandler;
            process.OutputDataReceived += OutputReceivedEventHandler;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return process;
        }
        
        private static void ErrorReceivedEventHandler(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e?.Data) == false)
            {
                _updateTextAction?.Invoke(e.Data);
                Log(LogType.Error, e.Data);
            }
        }
        
        private static void OutputReceivedEventHandler(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e?.Data) == false)
            {
                _updateTextAction?.Invoke(e.Data);
            }
        }
        
        [MenuItem("Gamesture/Code Docs/Open")]
        public static void OpenDocs()
        {
            StartCommand(CodeDocsSettings.OpenDocsExe, PlayerSettings.productName);
        }

        [MenuItem("Gamesture/Code Docs/Open", true)]
        public static bool OpenDocs_Validate()
        {
            return CodeDocsSettings.AreDocsGenerated();
        }

        private static bool IsMainThread()
        {
            return Thread.CurrentThread == _mainThread;
        }

        private static void Log(LogType type, string message)
        {
            void LogWithUnity() => UnityEngine.Debug.unityLogger.Log(type, $"[{nameof(CodeDocsGenerator)}] {message}");

            if (IsMainThread())
            {
                LogWithUnity();
            }
            else
            {
                lock (_actions)
                {
                    _actions.Enqueue(LogWithUnity);
                }
            }
        }

        [MenuItem("Gamesture/Code Docs/Generate and open")]
        public static void GenerateAndOpenDocs()
        {
            if (CodeDocsSettings.IsProperlyConfigured() == false)
            {
                CodeDocsSettings.ShowConfiguration();
                return;
            }
            
            _shouldClosePopup = false;
            Process generateProcess = StartCommand(CodeDocsSettings.GenerateDocsExe, PlayerSettings.productName,
                CodeDocsSettings.SourcesPath);
            CodeDocsInfoPopup.Show(out _updateTextAction, out _closePopup);
            
            EditorApplication.update += Update;
            new Thread(() =>
            {
                Log(LogType.Log, "thread started");

                if (generateProcess == null)
                {
                    Log(LogType.Error, "Could not start generate process");
                    _shouldClosePopup = true;
                    return;
                }

                Log(LogType.Log, "waiting for thread to finish");
                generateProcess.WaitForExit();
                
                Log(LogType.Log, "process finished");
                if (generateProcess.ExitCode == 0)
                {
                    EditorApplication.delayCall += OpenDocs;
                }

                _shouldClosePopup = true;
            }).Start();
        }

        [MenuItem("Gamesture/Code Docs/Modules")]
        public static void Modules()
        {
            if (CodeDocsSettings.IsProperlyConfigured() == false)
            {
                CodeDocsSettings.ShowConfiguration();
            }
            else
            {
                CodeDocsModulesPopup.Init();
            }
        }
        
        [MenuItem("Gamesture/Code Docs/Configure")]
        public static void OpenConfig()
        {
            CodeDocsSettings.ShowConfiguration();
        }
    }
}