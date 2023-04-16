namespace Installers.UI
{
    using MainHandlers.UI;
    using System;
    using Sirenix.OdinInspector;
    using UnityEngine;
    using Zenject;

    public class UIBindInstaller : MonoInstaller
    {
        [SerializeField, HideLabel, LabelText("UI SETTINGS")]
        private UISettings settings;

        public override void InstallBindings()
        {
            InstallUIBindings();
            InstallSettingsInstances();
        }


        private void InstallUIBindings()
        {
            Container.BindInterfacesTo<UIManager>().AsSingle();
            Container.BindInterfacesTo<ChapterText>().AsSingle();
        }

        private void InstallSettingsInstances()
        {
            Container.BindInstance(settings.UIBase).AsSingle();
            Container.BindInstance(settings.ChapterText).AsSingle();
        }

        [Serializable]
        private class UISettings
        {
            [TabGroup("Base UI Settings")] [HideLabel]
            public UIManager.BaseSettings UIBase;

            public ChapterText.UISettings ChapterText;
        }
    }
}