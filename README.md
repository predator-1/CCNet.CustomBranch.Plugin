# Usage
You need to add some configs in your project and CCnet

# CruiseControl.NET console
 * Add ccnet.custombranchcore.plugin.dll in main path.
 * Add in ccnet.exe.config
 ```
<cruiseServer>
  <extension type="ccnet.custombranchcore.plugin.CustomBranchPlugin,ccnet.custombranchcore.plugin" />
</cruiseServer>
```


# CruiseControl.NET WebDashboard
 * Add CCNet.CustomBranch.Plugin.dll in \bin
 * Add in dashboard.config
```
<projectPlugins>
      ....
      <customBranchPlugin />
</projectPlugins>
```


# Include project config
 * Add in your config
```
<labeller type="getBranches">
  <branches>
  </branches>
</labeller>
```
