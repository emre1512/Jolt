# Jolt

![https://msdn.microsoft.com/en-us/library/w0x726c2(v=vs.110).aspx](https://img.shields.io/badge/platform-C%23%2F.NET-brightgreen.svg?style=flat-square)
![https://www.apache.org/licenses/LICENSE-2.0](https://img.shields.io/badge/licence-Apache%20v2.0-blue.svg?style=flat-square)
![https://github.com/emre1512/Jolt](https://img.shields.io/badge/version-0.3.5-ff69b4.svg?style=flat-square)

## What Jolt can do?

- Asynchronous GET,POST,PUT,DELETE,HEAD requests

	* Asynchronously Download (via GET)
		* Text
                * JSON
		* Files
	* Asynchronously Upload (via POST/PUT)
		* Text
                * JSON
		* application/x-www-form-urlencoded
		* multipart/form-data
		* Files

- Asynchronously FTP Download/Upload

- Authentication

- Custom cookies

- Progress reports for upload/download

- All requests/operations return optional callbacks (OnStart, OnComplete, OnFail, OnProgress) 


## Installation

- Download the latest release of [Jolt.dll](https://github.com/emre1512/Jolt/releases/latest) and add it as refference to your project.
- Remember to add ```using JoltHttp``` namespace when using.

## Usage

### Get JSON

```cs
Jolt.GET("Url").AsJSON().MakeRequest(
    OnComplete: (Result) =>
    {
        // do something...
    }
);
```

### Get file

```cs
Jolt.GET("Url").AsFile().SaveTo(@"C:\image.jpg").MakeRequest(
    OnComplete: () =>
    {
        // do something...
    }
);
```

### Post JSON

```cs
Jolt.POST("Url").AsJSON(YourJsonObject).MakeRequest(
    OnComplete: (Result) =>
    {
        // do something...
    }
);
```

### Post form

```cs
Jolt.POST("Url").AsForm()
    .AddField("username", "Bob")
    .AddField("foo", "boo")
    .MakeRequest(
    OnComplete: (Result) =>
    {
        // do something...
    }
);
```

### Post multipart data

```cs
Jolt.POST("Url").AsMultipart().
    .AddField("foo", "boo")
    .AddFile(@"C:\image.jpg")
    .MakeRequest(
    OnComplete: (Result) =>
    {
        // do something...
    }
);
```

### Upload file via PUT

```cs
Jolt.PUT("Url").AddFile(@"C:\image.jpg").MakeRequest(
    OnComplete: () =>
    {
        // do something...
    }
);
```

### Upload file to ftp

```cs
Jolt.Upload("Url")
    .AddFile(@"C:\image.jpg")
    .SetCredentials("username", "password")
    .MakeRequest(
    OnComplete: () =>
    {
        // do something...
    }
);
```

### Download file from ftp

```cs
Jolt.Download("Url")
    .SaveTo(@"C:\image.jpg")
    .SetCredentials("username", "password")
    .MakeRequest(
    OnComplete: () =>
    {
        // do something...
    }
);
```

- Detailed documentation of other features will be added soon

## Detailed Documentation

- Detailed documentation can be found at [Wiki](https://github.com/emre1512/Jolt/wiki)

## LICENSE

Copyright 2016 M. Emre Davarci

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
