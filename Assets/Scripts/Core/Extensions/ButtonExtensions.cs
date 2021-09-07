using UnityEngine.UIElements;

namespace Core.Extensions {
    public static class ButtonExtensions {
        /// <summary>
        /// Event extension to blur the button after it has been clicked, to remove eventual errors that can occur when changing scenes
        /// </summary>
        /// <param name="button">The button that registers event</param>
        /// <param name="callback">callback that's being called on event</param>
        /// <param name="useTrickeDown">How the event handling acts between child and parent, which should be called first</param>
        /// <typeparam name="TEventType">What type of event</typeparam>
        public static void On<TEventType>(this Button button, EventCallback<TEventType> callback,
            TrickleDown useTrickeDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new() {
            if (typeof(TEventType).IsAssignableFrom(typeof(ClickEvent))) {
                EventCallback<TEventType> middleware = evt => {
                    button?.focusController?.focusedElement?.Blur();
                };

                middleware += callback; 
                
                button.RegisterCallback(middleware , useTrickeDown);
            } else {
                button.RegisterCallback(callback , useTrickeDown);   
            }
        }
    }
}