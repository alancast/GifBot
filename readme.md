# About GifBot

GifBot is a bot that will provide you a gif at random from the string that you pass it. It can be connected to a variety of things, but the primary (and most tested) use case is in Microsoft Teams. It uses the Giphy API as the source of it's GIFs.

# Azure boilerplate how to use

## Use Azure app service editor

1. make code change in the online editor
2. open the console window and run

```
build.cmd
```

## Use Visual Studio 

### Build and debug
1. download source code zip and extract source in local folder
2. open {PROJ_NAME}.sln in Visual Studio
3. build and run the bot
4. download and run [botframework-emulator](https://emulator.botframework.com/)
5. connect the emulator to http://localhost:3987

### Publish back

In Visual Studio, right click on {PROJ_NAME} and select 'Publish'

For first time publish after downloading source code
1. In the publish profiles tab, click 'Import'
2. Browse to 'PostDeployScripts' and pick '{SITE_NAME}.publishSettings'


## Use continuous integration

If you have setup continuous integration, then your bot will automatically deployed when new changes are pushed to the source repository.



