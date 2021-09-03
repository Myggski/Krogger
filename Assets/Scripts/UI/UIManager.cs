using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace FG {
	[RequireComponent(typeof(UIDocument))]
	public abstract class UIManager<T> : MonoBehaviour {
		protected static T instance;
		protected UIDocument document;
		protected VisualElement rootElement;
		
		/// <summary>
		/// Making sure that there's only one of this component
		/// </summary>
		private void InitializeManager() {
			if (!ReferenceEquals(instance, null) && !ReferenceEquals(instance, this)) {
				Destroy(gameObject);
			}
			else {
				instance = (T)Convert.ChangeType(this, typeof(T));
				document = GetComponent<UIDocument>();
			}
		}
		
		/// <summary>
		/// Hides button from the menu by adding a class to the button
		/// </summary>
		/// <param name="element">It can be either Play- or Resume-button</param>
		protected void HideElement(VisualElement element) {
			element.AddToClassList("hidden");
		}

		/// <summary>
		/// Show button from the menu by removing a class from the button
		/// </summary>
		/// <param name="element">It can be either Play- or Resume-button</param>
		protected void ShowElement(VisualElement element) {
			element.RemoveFromClassList("hidden");
		}

		/// <summary>
		/// Method to override, to set all the elements and is being called in OnEnable
		/// </summary>
		protected virtual void InitializeElements() {}

		/// <summary>
		/// Method to override, to remove all the click-listeners and is being called in OnDisable
		/// </summary>
		protected virtual void RemoveClickEvents() {}
		
		protected virtual void Awake() {
			InitializeManager();
		}

		protected virtual void OnEnable() {
			InitializeElements();
		}

		protected virtual void OnDisable() {
			RemoveClickEvents();
		}
	}
}
