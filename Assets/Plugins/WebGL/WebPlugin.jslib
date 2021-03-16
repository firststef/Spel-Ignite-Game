mergeInto(LibraryManager.library, {
  RequestAction: function () {
    ReactUnityWebGL.RequestAction();
  },
  GamePaused: function () {
    ReactUnityWebGL.GamePaused();
  },
});