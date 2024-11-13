using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CharacterUISelectionController : MonoBehaviour
{
    [SerializeField] private GameObject _playerPrefab;


    [SerializeField] private List<CharacterRaceTemplate> _races;
    [SerializeField] private List<CharacterClassTemplate> _classes;
    [SerializeField] private List<WeaponTemplate> _weapons;
    [SerializeField] private List<ArmourTemplate> _armours;
    [SerializeField] private List<TrinketTemplate> _trinkets;


    private int _currentRaceIndex = 0;
    private int _currentClassIndex = 0;
    private int _currentWeaponIndex = 0;
    private int _currentArmourIndex = 0;
    private int _currentTrinketIndex = 0;


    [SerializeField] private TMP_Text _raceNameText, _raceStatsText;
    [SerializeField] private TMP_Text _classNameText, _classStatsText;
    [SerializeField] private TMP_Text _weaponNameText, _weaponStatsText;
    [SerializeField] private TMP_Text _armourNameText, _armourStatsText;
    [SerializeField] private TMP_Text _trinketNameText, _trinketStatsText;


    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _nextRaceButton, _prevRaceButton;
    [SerializeField] private Button _nextClassButton, _prevClassButton;
    [SerializeField] private Button _nextWeaponButton, _prevWeaponButton;
    [SerializeField] private Button _nextArmourButton, _prevArmourButton;
    [SerializeField] private Button _nextTrinketButton, _prevTrinketButton;


    private void Start()
    {
        ShowRace(_currentRaceIndex);
        ShowClass(_currentClassIndex);
        ShowWeapon(_currentWeaponIndex);
        ShowArmour(_currentArmourIndex);
        ShowTrinket(_currentTrinketIndex);

        _nextRaceButton.onClick.AddListener(NextRace);
        _prevRaceButton.onClick.AddListener(PreviousRace);
        _nextClassButton.onClick.AddListener(NextClass);
        _prevClassButton.onClick.AddListener(PreviousClass);
        _nextWeaponButton.onClick.AddListener(NextWeapon);
        _prevWeaponButton.onClick.AddListener(PreviousWeapon);
        _nextArmourButton.onClick.AddListener(NextArmour);
        _prevArmourButton.onClick.AddListener(PreviousArmour);
        _nextTrinketButton.onClick.AddListener(NextTrinket);
        _prevTrinketButton.onClick.AddListener(PreviousTrinket);
        _confirmButton.onClick.AddListener(ConfirmSelection);
    }


    private void ShowRace(int index)
    {
        CharacterRaceTemplate selectedRace = _races[index];

        _raceNameText.text = selectedRace.Name;
        _raceStatsText.text = FormatStats(selectedRace.Stats);
    }
    private void NextRace()
    {
        _currentRaceIndex = (_currentRaceIndex + 1) % _races.Count;
        ShowRace(_currentRaceIndex);
    }
    private void PreviousRace()
    {
        _currentRaceIndex = (_currentRaceIndex - 1 + _races.Count) % _races.Count;
        ShowRace(_currentRaceIndex);
    }
    private void ShowClass(int index)
    {
        CharacterClassTemplate selectedClass = _classes[index];

        _classNameText.text = selectedClass.Name;
        _classStatsText.text = FormatStats(selectedClass.Stats);
    }
    private void NextClass()
    {
        _currentClassIndex = (_currentClassIndex + 1) % _classes.Count;
        ShowClass(_currentClassIndex);
    }
    private void PreviousClass()
    {
        _currentClassIndex = (_currentClassIndex - 1 + _classes.Count) % _classes.Count;
        ShowClass(_currentClassIndex);
    }
    private void ShowWeapon(int index)
    {
        WeaponTemplate selectedWeapon = _weapons[index];

        _weaponNameText.text = selectedWeapon.Name;
        _weaponStatsText.text = FormatStats(selectedWeapon.Stats);
    }
    private void NextWeapon()
    {
        _currentWeaponIndex = (_currentWeaponIndex + 1) % _weapons.Count;
        ShowWeapon(_currentWeaponIndex);
    }
    private void PreviousWeapon()
    {
        _currentWeaponIndex = (_currentWeaponIndex - 1 + _weapons.Count) % _weapons.Count;
        ShowWeapon(_currentWeaponIndex);
    }
    private void ShowArmour(int index)
    {
        ArmourTemplate selectedArmour = _armours[index];

        _armourNameText.text = selectedArmour.Name;
        _armourStatsText.text = FormatStats(selectedArmour.Stats);
    }
    private void NextArmour()
    {
        _currentArmourIndex = (_currentArmourIndex + 1) % _armours.Count;
        ShowArmour(_currentArmourIndex);
    }
    private void PreviousArmour()
    {
        _currentArmourIndex = (_currentArmourIndex - 1 + _armours.Count) % _armours.Count;
        ShowArmour(_currentArmourIndex);
    }

    private void ShowTrinket(int index)
    {
        TrinketTemplate selectedTrinket = _trinkets[index];

        _trinketNameText.text = selectedTrinket.Name;
        _trinketStatsText.text = FormatStats(selectedTrinket.Stats);
    }
    private void NextTrinket()
    {
        _currentTrinketIndex = (_currentTrinketIndex + 1) % _trinkets.Count;
        ShowTrinket(_currentTrinketIndex);
    }
    private void PreviousTrinket()
    {
        _currentTrinketIndex = (_currentTrinketIndex - 1 + _trinkets.Count) % _trinkets.Count;
        ShowTrinket(_currentTrinketIndex);
    }


    public CharacterRaceTemplate GetSelectedRace()
    {
        return _races[_currentRaceIndex];
    }
    public CharacterClassTemplate GetSelectedClass()
    {
        return _classes[_currentClassIndex];
    }
    public WeaponTemplate GetSelectedWeapon()
    {
        return _weapons[_currentWeaponIndex];
    }
    public ArmourTemplate GetSelectedArmour()
    {
        return _armours[_currentArmourIndex];
    }
    public TrinketTemplate GetSelectedTrinket()
    {
        return _trinkets[_currentTrinketIndex];
    }


    private string FormatStats(Stats stats)
    {
        return $"HP: {stats.Hp}\n" +
               $"Physical Damage: {stats.PhysicalDamage}\n" +
               $"Magical Damage: {stats.MagicalDamage}\n" +
               $"Movement Speed: {stats.MovementSpeed}\n" +
               $"Attack Speed: {stats.AttackSpeed}\n" +
               $"Physical Defense: {stats.PhysicalDefense}\n" +
               $"Magical Defense: {stats.MagicalDefense}\n" +
               $"Cd: {stats.CooldownReduction}";
    }
    public void ConfirmSelection()
    {
        GameObject playerInstance = Instantiate(_playerPrefab);

        CharacterRaceTemplate selectedRace = GetSelectedRace();
        CharacterClassTemplate selectedClass = GetSelectedClass();
        WeaponTemplate selectedWeapon = GetSelectedWeapon();
        ArmourTemplate selectedArmour = GetSelectedArmour();
        TrinketTemplate selectedTrinket = GetSelectedTrinket();

        Player playerScript = playerInstance.GetComponent<Player>();

        if (playerScript != null)
        {
            playerScript.BuildPlayer(selectedRace, selectedClass, selectedWeapon, selectedArmour, selectedTrinket, "YOU YOU");
        }

        DontDestroyOnLoad(playerInstance);
        SceneManager.LoadScene("PruebasDiego");

    }

}
