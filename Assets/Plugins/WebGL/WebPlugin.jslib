mergeInto(LibraryManager.library, {
  RequestAction: function (str) {
    var newStr = Pointer_stringify(str);
    ReactUnityWebGL.RequestAction(newStr);
  },
  GamePaused: function () {
    ReactUnityWebGL.GamePaused();
  },
  GameUnpaused: function () {
    ReactUnityWebGL.GameUnpaused();
  },
});