# BOSSWAVE Binding
## C# Base Class Library
**This is a** <span style="color:purple">**.NET Core Library**</span> **manifesting the BOSSWAVE 2.2.0 bindings for** <span style="color:purple">**C#**</span>**.**
<br>
*University of Southern Denmark*, Center for Energy Informatics.

## Instructions
First of all you have to install the latest release of BOSSWAVE.
<br>When that is done, configure that client to run as proposed in ClientImplementation.cs.
<br>Remember that BOSSWAVE runs on localhost.

**Install on Ubuntu:** <br>
If you're on windows use [WSL](https://www.howtogeek.com/249966/how-to-install-and-use-the-linux-bash-shell-on-windows-10/). 
<br>It is recommended that you install Ubuntu >= 16.04 from the Microsoft Store. 
<br>After install type bash in the commandline and execute the following:



```
curl get.bw2.io/agent | sudo bash
```
Upon reboot, BOSSWAVE doesn't automatically start. In that case you'll have to run it again:
```
sudo /etc/init.d/bw2 start
```

