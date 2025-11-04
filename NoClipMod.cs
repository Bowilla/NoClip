using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BBModMenu;
using MelonLoader;
using UnityEngine;
using UnityEngine.UIElements;

namespace NoClip
{
    public class NoClipMod : MelonMod
    {
        private CapsuleCollider collider;
        private string noclipKey;
        private bool toggle;

        public override void OnLateInitializeMelon()
        {
            MelonLogger.Msg("NoClip starting to load.");

            GameObject gameUI = GameObject.Find("GameUI");
            GameUI _gameUI = gameUI.GetComponent<GameUI>();
            List<UIScreen> screens = typeof(GameUI)?.GetField("screens", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.GetValue(_gameUI) as List<UIScreen>;

            ModMenu _modMenu = screens?.FirstOrDefault(screen => screen is ModMenu) as ModMenu;
            if (_modMenu is null)
            {
                MelonLogger.Msg("ModMenu not found");
                return;
            }

            string categoryName = "NoClip";
            var noClipSettings = _modMenu.AddSetting(categoryName);

            var enableToggle = _modMenu.CreateToggle(categoryName, "EnabledOnStart", true);

            var key = _modMenu.CreateHotKey(categoryName, "NoClipKey", KeyCode.N);
            noclipKey = key.Value;
            key.OnChanged += newKey =>
            {
                MelonLogger.Msg($"NoClip Key : {newKey}");
                noclipKey = newKey;
            };

            var togglesGroup = _modMenu.CreateGroup("Toggles");

            var toggleWrapper = _modMenu.CreateWrapper();
            toggleWrapper.Add(_modMenu.CreateLabel("Enabled on Start"));
            toggleWrapper.Add(enableToggle);

            var keyWrapper = _modMenu.CreateWrapper();
            keyWrapper.Add(_modMenu.CreateLabel("NoClip Toggle Key"));
            keyWrapper.Add(key.Root);

            togglesGroup.Add(toggleWrapper);
            togglesGroup.Add(keyWrapper);

            noClipSettings.Add(togglesGroup);


            collider = GameObject.FindObjectOfType<PlayerController>().GetComponent<CapsuleCollider>();

            toggle = !enableToggle.value;
        }



        public override void OnUpdate()
        {
            if (Utils.IsHotkeyPressed(noclipKey)) toggle = !toggle;

            if (GameModeManager.Instance.IsGameModeActive<MapEditorGameMode>() && GameModeManager.Instance.player.IsFlying)
            {
                collider.enabled = toggle;
            }
            else
            {
                collider.enabled = true;
            }
        }

    }
}
