mergeInto(LibraryManager.library, {
  UpdateInventory: function (str) {
    var newStr = Pointer_stringify(str);
    ReactUnityWebGL.UpdateInventory(newStr);
  },
  GamePaused: function () {
    ReactUnityWebGL.GamePaused();
  },
  GameUnpaused: function () {
    ReactUnityWebGL.GameUnpaused();
  },
  RequestAction: function () {
    ReactUnityWebGL.RequestAction();
  },
});