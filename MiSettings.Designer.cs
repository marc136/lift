﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Lift {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
    internal sealed partial class MiSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static MiSettings defaultInstance = ((MiSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new MiSettings())));
        
        public static MiSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("horst")]
        public string Executables {
            get {
                return ((string)(this["Executables"]));
            }
            set {
                this["Executables"] = value;
            }
        }
    }
}
