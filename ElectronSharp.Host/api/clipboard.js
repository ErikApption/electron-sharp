"use strict";
const electron_1 = require("electron");
let electronSocket;
module.exports = (socket) => {
    electronSocket = socket;
    socket.on('clipboard-readText', (type) => {
        const text = electron_1.clipboard.readText(type);
        electronSocket.emit('clipboard-readText-Completed', text);
    });
    socket.on('clipboard-writeText', (text, type) => {
        electron_1.clipboard.writeText(text, type);
    });
    socket.on('clipboard-readHTML', (type) => {
        const content = electron_1.clipboard.readHTML(type);
        electronSocket.emit('clipboard-readHTML-Completed', content);
    });
    socket.on('clipboard-writeHTML', (markup, type) => {
        electron_1.clipboard.writeHTML(markup, type);
    });
    socket.on('clipboard-readRTF', (type) => {
        const content = electron_1.clipboard.readRTF(type);
        electronSocket.emit('clipboard-readRTF-Completed', content);
    });
    socket.on('clipboard-writeRTF', (text, type) => {
        electron_1.clipboard.writeHTML(text, type);
    });
    socket.on('clipboard-readBookmark', () => {
        const bookmark = electron_1.clipboard.readBookmark();
        electronSocket.emit('clipboard-readBookmark-Completed', bookmark);
    });
    socket.on('clipboard-writeBookmark', (title, url, type) => {
        electron_1.clipboard.writeBookmark(title, url, type);
    });
    socket.on('clipboard-readFindText', () => {
        const content = electron_1.clipboard.readFindText();
        electronSocket.emit('clipboard-readFindText-Completed', content);
    });
    socket.on('clipboard-writeFindText', (text) => {
        electron_1.clipboard.writeFindText(text);
    });
    socket.on('clipboard-clear', (type) => {
        electron_1.clipboard.clear(type);
    });
    socket.on('clipboard-availableFormats', (type) => {
        try {
            const formats = electron_1.clipboard.availableFormats(type);
            electronSocket.emit('clipboard-availableFormats-Completed', formats);
        } catch (e) {
            electronSocket.emit('clipboard-availableFormats-Completed', []);
        }
    });
    socket.on('clipboard-write', (data, type) => {
        if (data.hasOwnProperty("image")) {
            data["image"] = deserializeImage(data["image"]);
        }
        electron_1.clipboard.write(data, type);
    });
    socket.on('clipboard-readImage', (type) => {
        try {
            const image = electron_1.clipboard.readImage(type);

            const imgBase64 = image.toPNG().toString('base64');
            if (imgBase64.length > 0) {
                electronSocket.emit('clipboard-readImage-Completed', {1: imgBase64});
            } else {
                electronSocket.emit('clipboard-readImage-Completed', {});
            }
        } catch (e) {
            electronSocket.emit('clipboard-readImage-Completed', {});
        }
    });
    socket.on('clipboard-writeImage', (data, type) => {
        const dataContent = JSON.parse(data);
        const image = deserializeImage(dataContent);
        electron_1.clipboard.writeImage(image, type);
    });

    function deserializeImage(data) {
        const image = electron_1.nativeImage.createEmpty();
        // tslint:disable-next-line: forin
        for (const key in data) {
            const scaleFactor = key;
            const bytes = data[key];
            const buffer = Buffer.from(bytes, 'base64');
            image.addRepresentation({scaleFactor: +scaleFactor, buffer: buffer});
        }
        return image;
    }
};
//# sourceMappingURL=clipboard.js.map