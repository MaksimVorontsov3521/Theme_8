﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Server.Settings {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.13.0.0")]
    internal sealed partial class Settings2 : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings2 defaultInstance = ((Settings2)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings2())));
        
        public static Settings2 Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("3")]
        public int CanCreateNewProject {
            get {
                return ((int)(this["CanCreateNewProject"]));
            }
            set {
                this["CanCreateNewProject"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("3")]
        public int CanEditClient {
            get {
                return ((int)(this["CanEditClient"]));
            }
            set {
                this["CanEditClient"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int BackupSchedule {
            get {
                return ((int)(this["BackupSchedule"]));
            }
            set {
                this["BackupSchedule"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int KeepBackups {
            get {
                return ((int)(this["KeepBackups"]));
            }
            set {
                this["KeepBackups"] = value;
            }
        }
    }
}
