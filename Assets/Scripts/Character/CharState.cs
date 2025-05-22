using UnityEngine;

public abstract class CharState
{
    protected Character character;

    public CharState(Character character)
    {
        this.character = character;
    }

    public abstract void EnterState();
    public abstract void UpdateState();

    public abstract void OnTriggerEnter2D(Collider2D collision);
    public abstract void OnTriggerExit2D(Collider2D collision);

}

public class NormalState : CharState
{
    public NormalState(Character character) : base(character) {}

    public override void EnterState()
    {
        Debug.Log("Entro a estado Normal");
        character.SetSpeed(character.normalSpeed);
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Fire"))
        {
            character.TransitionToState(new OnfireState(character));
        }
        else if (collision.gameObject.CompareTag("Water"))
        {
            character.TransitionToState(new SwimingState(character));
        }
    }

    public override void OnTriggerExit2D(Collider2D collision)
    {

    }

    public override void UpdateState()
    {

    }

}

public class OnfireState : CharState
{
    public OnfireState(Character character) : base(character) {}

    public override void EnterState()
    {
        Debug.Log("Entro a estado En llamas");
        character.animator.SetTrigger("Fire");
        character.SetSpeed(character.onfireSpeed);
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Water"))
        {
            character.TransitionToState(new SwimingState(character));
        }
    }

    public override void OnTriggerExit2D(Collider2D collision)
    {

    }

    public override void UpdateState()
    {

    }

}

public class SwimingState : CharState
{
    public SwimingState(Character character) : base(character) {}

    private int _rios;

    public override void EnterState()
    {
        Debug.Log("Entro a estado Nadando");
        character.SetSpeed(character.swimingSpeed);
        character.animator.SetTrigger("Roll-in");
        _rios = 1;
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Water"))
        {
            _rios ++;
        }
    }

    public override void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Water"))
        {
            _rios--;
            if (_rios == 1)
            {
                character.animator.SetTrigger("Roll-out");
                character.TransitionToState(new NormalState(character));
            }
        }
    }

    public override void UpdateState()
    {
        
    }

}