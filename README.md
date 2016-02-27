# PassiveEyes Windows 10 App
PassiveEyes UWP application, including the webcam client and remote control.

This was created in a little under two days for Microsoft's 2016 [//hackforher](http://hackforher.org/) hackathon in [The Garage](http://www.microsoft.com/en-us/garage/).

**PassiveEyes** is a home monitoring application for the Windows 10 universal platform. It monitors your webcam feed(s) and uploads photos to OneDrive whenever it detects a significant change. You can then see the uploaded photos in near-real-time using the app on the go.


### Development Status

Horrible.

This barely works for the purposes of demos, and needs a re-write. Use at your own risk.

#### Runtime Caveats

* Photos are hardcoded to upload to the `/PassiveEyes` folder in your OneDrive.
* The recording page never lets go of your cameras.
* It will crash occasionally.
* The viewer page always requests metadata on all photos, so it will refresh gradually slower over time. To resolve this, delete or move folders out of your PassiveEyes folder.


### Media

See the [Media repository](https://github.com/PassiveEyes/Media/) for promotional media.
