using UnityEngine;

public class Fireball : ExplosiveProjectile
{
    private void ScaleUp()
    {
        transform.localScale += Vector3.one * Time.deltaTime;
    }

    protected override void InitializeStates()
    {
        _fsm.Add(new FireballLiveState(this));
        _fsm.Add(new ProjectileImpactingState(this));
        _fsm.Add(new ProjectileDestroyedState(this));
    }

    private class FireballLiveState : ProjectileLiveState
    {
        readonly Fireball _projectile;
        public FireballLiveState(Fireball projectile) : base(projectile) => _projectile = projectile;

        public override void Update()
        {
            base.Update();

            _projectile.ScaleUp();
        }
    }
}