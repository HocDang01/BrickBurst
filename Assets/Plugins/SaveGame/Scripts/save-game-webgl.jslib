mergeInto(LibraryManager.library, {

  SaveDataJS: function (fileName, data) {
    var key = UTF8ToString(fileName);
    var value = UTF8ToString(data);

    if (typeof localStorage !== "undefined") {
      localStorage.setItem(key, value);
    }
  },

  ReadDataJS: function (dataName) {
    var key = UTF8ToString(dataName);
    var value = "";

    if (typeof localStorage !== "undefined") {
      value = localStorage.getItem(key) || "";
    }

    var bufferSize = lengthBytesUTF8(value) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(value, buffer, bufferSize);

    return buffer;
  },

  ExistJS: function (dataName) {
    var key = UTF8ToString(dataName);

    if (typeof localStorage === "undefined") return 0;

    return localStorage.getItem(key) !== null ? 1 : 0;
  },

  DeleteJS: function (dataName) {
    var key = UTF8ToString(dataName);

    if (typeof localStorage !== "undefined") {
      localStorage.removeItem(key);
    }
  },

  DeleteAllJS: function () {
    if (typeof localStorage !== "undefined") {
      localStorage.clear();
    }
  }
});