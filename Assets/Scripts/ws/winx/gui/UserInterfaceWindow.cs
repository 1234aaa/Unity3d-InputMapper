using System;
using UnityEngine;
using ws.winx.input;
using ws.winx.input.states;
using System.Collections.Generic;
using System.IO;

namespace ws.winx.gui
{
    
    public class UserInterfaceWindow:MonoBehaviour
    {
		protected Rect _buttonRect = new Rect (0, 0, 100, 15);
		protected Rect _layerLabelRect = new Rect (0, 0, 100, 15);
		protected static Dictionary<int,InputState> _stateInputCombinations;
		protected static bool _settingsLoaded=false;
		protected int _selectedStateHash = 0;
		protected string _combinationSeparator=InputAction.SPACE_DESIGNATOR.ToString();
		protected int _isPrimary = 0;
		protected string _currentInputString;
		protected GUILayoutOption[] _inputLabelStyle = new GUILayoutOption[]{ GUILayout.Width (200)};
		protected GUILayoutOption[] _stateNameLabelStyle = new GUILayoutOption[]{ GUILayout.Width (250)};
		protected InputAction _action;
		protected Vector2 _scrollPosition=Vector2.zero;
		protected InputCombination _previousStateInput = null;

		
		
		
		

		public int maxCombosNum = 3;
		public GUISkin guiSkin;
		public TextAsset settingsXML;
		//public bool allowDuplicates=false;

		
		/// <summary>
		/// Update this instance.
		/// </summary>
		void Update(){

			if (_selectedStateHash != 0)
			{
					_action = InputEx.GetInput();
			
			 if(_action != null && (_action.code ^ (int)KeyCode.Escape)!=0 && (_action.code^(int)KeyCode.Return)!=0) {
				
				
				if((_action.code^(int)KeyCode.Backspace)==0){
					_stateInputCombinations[_selectedStateHash].combinations[_isPrimary].Clear();
					_stateInputCombinations[_selectedStateHash].combinations[_isPrimary].Add(new InputAction(KeyCode.None));
				}
				else{
					toInputCombination (_stateInputCombinations [_selectedStateHash].combinations[_isPrimary], _action);
				}
				


					Debug.Log ("Action:"+_action+" "+_action.code);
				}
				
				
				//Debug.Log ("Action:"+action);
			}
			

		}


		/// <summary>
		/// Saves the input settings.
		/// </summary>
		void saveInputSettings ()
		{

			InputManager.saveSettings(Path.Combine(Application.streamingAssetsPath,settingsXML.name+".xml"));

		}


		/// <summary>
		/// Loads the input settings.
		/// </summary>
		void loadInputSettings(){

			//Path.Combine(Application.streamingAssetsPath, settingsXML.name+".xml");
			 
			_stateInputCombinations=InputManager.loadSettings(Path.Combine(Application.streamingAssetsPath,settingsXML.name+".xml")).stateInputs;
		}


		/// <summary>
		/// Tos the input combination.
		/// </summary>
		/// <param name="combos">Combos.</param>
		/// <param name="input">Input.</param>
		void toInputCombination (InputCombination combos, InputAction input)
		{

			    if(combos.numActions+1 > maxCombosNum || (combos.numActions==1 && combos.GetActionAt(0).code==0))
				     combos.Clear();

					combos.Add(input);
					
			
		}

		/// <summary>
		/// Raises the GU event.
		/// </summary>
		private void OnGUI()
		{
			GUI.skin=guiSkin;

            GUI.Window(1, new Rect(0, 0, 600, 400), CreateWindow, new GUIContent());
			//GUI.Window(1, new Rect(0, 0, Screen.width, Screen.height), CreateWindow,new GUIContent());


			//if event is of key or mouse
			if (Event.current.isKey) {
				


				if (Event.current.keyCode == KeyCode.Return) {
					_selectedStateHash = 0;
					_previousStateInput = null;
					//this.Repaint ();
				} else
				if (Event.current.keyCode == KeyCode.Escape) {
					if (_selectedStateHash != 0) {
						_stateInputCombinations [_selectedStateHash].combinations[_isPrimary] = _previousStateInput;
						_previousStateInput = null;
						_selectedStateHash = 0;
					}
				}





			}

			//Approach dependent of GUI so not applicable if you have 3D GUI
			//if (_selectedStateHash != 0)
			//	InputEx.processGUIEvent (Event.current);//process input from keyboard & mouses
           
        }


		/// <summary>
		/// Creates the window.
		/// </summary>
		/// <param name="windowID">Window I.</param>
        private void CreateWindow(int windowID)
        {

			GUILayout.Label("http://unity3de.blogspot.com/");

			GUILayout.Label("InputEx");

				
				if(!_settingsLoaded && settingsXML!=null) 
				{ 
					loadInputSettings();
					_settingsLoaded=true;
				}


					_scrollPosition=GUILayout.BeginScrollView(_scrollPosition,false,true);
			        
					

			if(_stateInputCombinations!=null)
			foreach (var keyValuPair in _stateInputCombinations)
			{
				//primary,secondary...
				createCombinationGUI(keyValuPair.Key,keyValuPair.Value.name,keyValuPair.Value.combinations);

			}
					
					GUILayout.EndScrollView ();
					
				
				
				
				GUILayout.Space(20);
				
				if(_selectedStateHash==0 && GUILayout.Button("Save")){
					saveInputSettings();
				}
				
			}




		/// <summary>
		/// Creates the combination GU.
		/// </summary>
		/// <param name="hash">Hash.</param>
		/// <param name="combinations">Combinations.</param>
		void createCombinationGUI (int hash,string stateName,InputCombination[] combinations)
		{

			string currentCombinationString;


			GUILayout.BeginHorizontal ();

			//string stateName=((CharacterInputControllerClass.States)hash).ToString();



				//(AnimatorEnum)hash
			//GUILayout.Label(stateName.Remove(0,stateName.IndexOf("Layer")+6).Replace("_"," "),_stateNameLabelStyle);

			
			
			if (_selectedStateHash != hash) {
				
				

				if (GUILayout.Button (combinations[0].combinationString)) {
					_selectedStateHash = hash;
					_previousStateInput = null;
					_isPrimary = 0;
				}

				if(combinations.Length>1 && combinations[1]!=null)
				if (GUILayout.Button (combinations[1].combinationString)) {
					_selectedStateHash = hash;
					_previousStateInput = null;
					_isPrimary = 1;
				}
				
				
			} else {

				currentCombinationString = combinations[_isPrimary].combinationString;

				if (_previousStateInput == null) {
					_previousStateInput = combinations[_isPrimary].Clone();
				}


				GUILayout.Label (currentCombinationString);//, _inputLabelStyle);

				//this.Repaint ();
			}
			
			
			
			//Debug.Log ("_selectedStateHash after" + _selectedStateHash);
			
			
			
			GUILayout.EndHorizontal ();
			
			
			
			GUILayout.Space(20);
		}

        
    }
}