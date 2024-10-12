const { app, protocol,BrowserWindow, net } = require('electron')
const path = require('node:path')
const url = require('node:url')



const createWindow = () =>{
    const win = new BrowserWindow({
        width: 800,
        height: 600,
        webPreferences: {
            allowRunningInsecureContent: true
        }

    })
    win.loadFile('./build/index.html')

}

protocol.registerSchemesAsPrivileged([
    {
        scheme: 'BDPApp',
        privileges: {
            standard: true,
            secure: true,
            supportFetchAPI: true
        }
    }
])

app.whenReady().then(()=>{
    protocol.handle('BDPApp', (request) => {
        const filePath = request.url.slice('BDPApp://'.length)
        return net.fetch(url.pathToFileURL(path.join(__dirname, filePath)).toString())
    })

    createWindow()

    app.on('activate', () => {
        if (BrowserWindow.getAllWindows().length === 0) createWindow()
    })
})

app.on('window-all-closed', () => {
    if (process.platform !== 'darwin') app.quit()

})