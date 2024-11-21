using UnityEngine.SceneManagement;

namespace ForgottenTyrants
{
   public sealed class Scene
   {
      public const string StartMenu = "StartMenu";
      public const string PruebasIvan = "PruebasIvan";
      public const string PruebasDiego = "PruebasDiego";
      public const string UICharacterSelection = "UI Character Selection";

      public static int Next => GetNext();
      public static int Previous => GetPrevious();

      private static int GetNext()
      {
         return SceneManager.GetActiveScene().buildIndex + 1;
      }

      private static int GetPrevious()
      {
         return SceneManager.GetActiveScene().buildIndex + 1;
      }
   }
}
