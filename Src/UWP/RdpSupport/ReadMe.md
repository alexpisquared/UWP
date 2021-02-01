https://techcommunity.microsoft.com/t5/ask-the-directory-services-team/manage-developer-mode-on-windows-10-using-group-policy/ba-p/400312
Using Group Policy Editor (gpedit.msc), a developer mode can be enabled or disabled on computers running Windows 10.

1. Open the Windows Run box using keyboard, press Windows logo key‌thumbnail image 5 of blog post titled 

2. Type in gpedit.msc and then press Enter .

3. In Group Policy Editor navigate to Computer Configuration\Administrative Templates\Windows Components\App Package Deployment .

4. From the right-side pane, double click on Allow all trusted apps to install and click on Enabled button.

5. Click on Apply and then OK .

Notes:

· Allow all trusted apps to install

o If you want to disable access to everything in for developers’ disable this policy setting.

o If you enable this policy setting, you can install any LOB or developer-signed Windows Store app.

If you want to allow side-loading apps to install but disable the other options in developer mode disable "Developer mode" and enable "Allow all trusted apps to install"

· Group policies are applied every 90 minutes, plus or minus a random amount up to 30 minutes. To apply the policy immediately, run gpupdate from the command prompt.

For more information on Developer Mode, see the following MSDN article:
https://msdn.microsoft.com/library/windows/apps/xaml/dn706236.aspx?f=255&MSPPError=-2147217396 ...


https://windowsloop.com/enable-developer-mode-windows-10/
