using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace FG {
	[RequireComponent(typeof(UIDocument))]
	public abstract class UIManagerBase<T> : ManagerBase<T> {
		// The UIToolkit document
		protected UIDocument Document;
		// The root element of the UI
		protected VisualElement RootElement;
		
		// Scene name where the instance were instantiated 
		private string InstanceSceneName => gameObject.scene.name;

		/// <summary>
		/// Gets UIDocoument-component
		/// </summary>
		private void Setup() {
			Document = GetComponent<UIDocument>();
		}
		
		/// <summary>
		/// Hides button from the menu by adding a class to the button
		/// </summary>
		/// <param name="element">It can be either Play- or Resume-button</param>
		/// /// <param name="visibilityStyleType">Hide it by changing display, or opacity</param>
		protected void HideElement(VisualElement element, VisibilityStyleType visibilityStyleType = VisibilityStyleType.display) {
			if (visibilityStyleType == VisibilityStyleType.display) {
				element.AddToClassList("hidden");	
			} else {
				element.style.opacity = 0f;
			}
			
		}

		/// <summary>
		/// Show button from the menu by removing a class from the button
		/// </summary>
		/// <param name="element">It can be either Play- or Resume-button</param>
		/// /// <param name="visibilityStyleType">Show it by changing display, or opacity</param>
		protected void ShowElement(VisualElement element, VisibilityStyleType visibilityStyleType = VisibilityStyleType.display) {
			if (visibilityStyleType == VisibilityStyleType.display)
			{
				element.RemoveFromClassList("hidden");	
			}
			else {
				element.style.opacity = 1f;
			}
			
		}

		/// <summary>
		/// Uses UnloadSceneAsync because UnloadScene is obsolete
		/// We don't need for the operator to be done in this case, because it's such small scenes that are being loaded
		/// </summary>
		protected void UnloadSelfScene() {
			SceneManager.UnloadSceneAsync(InstanceSceneName);
		}

		/// <summary>
		/// Loading a scene Addictive. Adds "partial" scene to current scene
		/// </summary>
		/// <param name="scene">The name of the scene that's being loaded</param>
		protected void LoadSceneAdditively(string scene) {
			SceneManager.LoadScene(scene, LoadSceneMode.Additive);
		}

		/// <summary>
		/// Method to override, to set all the elements and is being called in OnEnable
		/// </summary>
		protected virtual void InitializeElements() {}

		/// <summary>
		/// Method to override, to remove all the click-listeners and is being called in OnDisable
		/// </summary>
		protected virtual void RemoveClickEvents() {}

		protected override void Awake() {
			base.Awake();

			Setup();
		}

		protected virtual void OnEnable() {
			InitializeElements();
		}

		protected virtual void OnDisable() {
			RemoveClickEvents();
		}
	}
}
