mergeInto(LibraryManager.library, {

  SaveDataJS: function (fileName, data) {
    var parsedFileName = UTF8ToString(fileName);
    var parsedData = UTF8ToString(data);

    saveGameData(parsedFileName, parsedData);
  },

  ReadDataJS: function (dataName) {
    var parsedDataName = UTF8ToString(dataName);
    var gameData = getGameData(parsedDataName);
    var bufferSize = lengthBytesUTF8(gameData) + 1;
    var buffer = _malloc(bufferSize);

    stringToUTF8(gameData, buffer, bufferSize);

    return buffer;
  },

  ExistJS: function (dataName) {
    var parsedDataName = UTF8ToString(dataName);
    
    return dataExist(parsedDataName);
  },

  DeleteJS: function (dataName) {
    var parsedDataName = UTF8ToString(dataName);
    
    deleteGameData(parsedDataName);
  },

  DeleteAllJS: function () {
    deleteAllGameData();
  }
})
