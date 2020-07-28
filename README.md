Swate
=====

> **Swate** - something or someone that gets you absolutely joyed([Urban dictionary](https://www.urbandictionary.com/define.php?term=swate))

**Swate** is a **S**wate **W**orkflow **A**nnotation **T**ool for **E**xcel.

_This project is in an exerimental state._



The aims of Swate are:

1. Provide an easy way to annotate experimental data in an application (excel) that every wet lab scientist is familiar with
2. Provide a way to create computational workflows that start from raw data and end with results.
3. Create ISA and CWL compatible data models from the input
4. Provide an interface for remote execution of the defined workflows

Check the state of the [minimal POC milestone ](https://github.com/nfdi4plants/Swate/milestone/1) for the current state of features.

Test
----

We are excited to announce that you can now test SWATE in a very early alpha preview version.
If you decide to do so, please take the time to set up a Github account to report your issues and suggestions [here](https://github.com/nfdi4plants/Swate/issues).
You can also search existing issues for solutions for your questions and/or discussions about your suggestions.

**Here are the necessary steps to use SWATE:**

- Excel (rightfully) enforces https on all addins. We currently have only a self-signed certificate. 
This means that you will have to install our certificate under `Trusted Root Certification Authorities`. You will need to do this for either of the following methods.
You can find the .cert file in the test release archive.
We recommend you to remove this certificate from your trusted sources when SWATE has an own certificate. 

 - **If you use Excel locally**:
   - Install [node.js LTS](https://nodejs.org/en/) (needed for office addin related tooling)
   - Download the [latest test release archive](https://github.com/nfdi4plants/Swate/releases) and extract it
   - If not done already, install `Swate_SelfSigned_Certificate.cer` under `Trusted Root Certification Authorities`
   - Execute the test.cmd (windows, as administrator) or test.sh (macOS, you will need to make it executable via chmod a+x) script.

 - **If you use Excel in the browser**:
   - Download the [latest test release archive](https://github.com/nfdi4plants/Swate/releases) and extract it
   - If not done already, install `Swate_SelfSigned_Certificate.cer` under `Trusted Root Certification Authorities`
   - Launch Excel online, open a (blank) workbook 
   - Under the `Insert` tab, select `Add-Ins`
   - Go to `Manage my Add-Ins` and select `Upload my Add-In`
   - select and upload the `manifest.xml` file contained in the test release archive.

Discuss
-------

Extensive documentation of Swate will be setup when the [development process of a minimal POC](https://github.com/nfdi4plants/Swate/milestone/1) is complete.

Please use Github issues to track problems, feature requests, questions, and discussions. Additionally, you can get in touch with us on [Twitter](https://twitter.com/nfdi4plants)

Develop
-------

#### Prerequisites:

 - .NET Core SDK at of at least the version in [the global.json file](global.json)
 - Node.js with npm/npx
 - To setup all dev dependencies, you can run the following commands:

    `dotnet tool restore` (to restore local dotnet tools)

    `dotnet fake build -t setup`, which will take care of installing necessary certificates and loopback exempts for your browser. Here are the steps if you want to execute them by yourself:

    - connections from excel to localhost need to be via https, so you need a certificate and trust it. [office-addin-dev-certs](https://www.npmjs.com/package/office-addin-dev-certs?activeTab=versions) does that for you.

        you can also use the fake build target for certificate creation and installation by using `dotnet fake build -t createdevcerts` in the project root (after restoring tools via `dotnet tool restore`).

        This will use office-addin-dev-certs to create the necessary certificates, and open the installation dialogue for you:

        ![File](docsrc/files/img/file.png)

        installing this ca certificate under your trusted root certification authorities will enable you to use httpss via localhost.

     - You may need a loopback exemption for Edge/IE (whatever is run in your excel version): 

        `CheckNetIsolation LoopbackExempt -a -n="microsoft.win32webviewhost_cw5n1h2txyewy"`

This project uses the [SAFE Stack](https://github.com/SAFE-Stack) to create a website that uses [office.js](https://github.com/OfficeDev/office-js) to interop with Excel.

The file [OfficeJS.fs](src/Client/OfficeJS.fs) contains Fable bindings autogenerated from the office.js typescript definitions.

to debug the AddIn locally, use the build target `OfficeDebug`:

`fake build -t OfficeDebug`

this will launch an Excel instance with the AddIn sideloaded.

However it is currently pretty hard to attach a debugger to the instance of edge that runs in
the Excel window (update: you can now use [EdgeDevToolsPreview](https://www.microsoft.com/en-us/p/microsoft-edge-devtools-preview/9mzbfrmz0mnj?activetab=pivot:overviewtab) for that aswell). You can circumvent this issue by additionally testing in Excel online:

 - open Excel online in your favorite browser
 - create a blank new workbook (or use any workbook, but be warned that you can't undo stuff done via Office.js) 
 - Go to Insert > Office-Add-Ins and upload the manifest.xml file contained in this repo
    ![Add In Upload](docsrc/files/img/AddInUpload.png)
 - You will now have the full debug experience in your browser dev tools.

Alternatively, you can debug all functionality that does not use Excel Interop in your normal browser (the app runs on port 3000 via https)