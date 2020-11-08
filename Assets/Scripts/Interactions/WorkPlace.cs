using UnityEngine;
using UnityEngine.Assertions;

public class WorkPlace : MonoBehaviour
{
    public float computerDirection = 90.0f;
    public Transform navMeshDestination;
    public Chair chair;
    public Computer computer;
    
    
    private bool _isOccupied = false;
    private bool _isVictimVulnerable = false;
    private float _passwordTypingStart;
    private float _passwordTypingTime;
    private Personality _workingPerson;
    private NoteSpawner _noteSpawner;
    private PasswordPeekingActivator _passwordPeekingActivator;

    public bool IsOccupied => _isOccupied;

    public bool IsVictimVulnerable => _isVictimVulnerable;

    public bool IsLoggedInCorrectProfile
    {
        get
        {
            string login;
            if (computer.IsLoggedIn(out login))
            {
                if (login == _workingPerson.LogInId)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public float PasswordTypingStart => _passwordTypingStart;

    private void Start()
    {
        _noteSpawner = GetComponent<NoteSpawner>();
    }

    public void SpawnExternalNote(string text)
    {
        _noteSpawner.SpawnHiddenNote(text);
    }

    public Personality WorkerOnSite
    {
        get
        {
            Assert.IsTrue(_isOccupied);
            Assert.IsNotNull(_workingPerson);
            return _workingPerson;
        }
    }
    
    public bool IsChairFacingComputer()
    {
        return chair.IsChairAllignedWith(Quaternion.AngleAxis(computerDirection, Vector3.up));
    }
    
    public void TurnToComputer()
    {
        chair.Turn(computerDirection);
    }
    
    public bool CharacterCanStandUp()
    {
        var checkingSpherePos = navMeshDestination.position + Vector3.up * 0.5f;
        return !(Physics.CheckSphere(checkingSpherePos, 0.5f, LayerMask.GetMask("Furniture")));
    }

    public void CharacterSeated(Personality person)
    {
        _workingPerson = person;
        _isOccupied = true;
    }

    public void CharacterStoodUp()
    {
        _isOccupied = false;
        _workingPerson = null;
    }

    public void StartTypingPassword(float passwordTime)
    {
        _passwordTypingStart = Time.time;
        _passwordTypingTime = passwordTime;
        _isVictimVulnerable = true;
    }

    public void StopTypingPassword()
    {
        _isVictimVulnerable = false;
    }
    public void LogIn(string id, string password, out bool rightPassword)
    {
        computer.ChooseProfile(id);
        computer.CheckPassword(password, out rightPassword);
    }

    public void LogOut()
    {
        computer.LogOut();
    }
    
}
