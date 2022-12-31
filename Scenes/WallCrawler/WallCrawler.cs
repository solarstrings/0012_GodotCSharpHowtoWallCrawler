using Godot;
using System;

public class WallCrawler : KinematicBody2D
{
    [Export]
    private float Speed = 100.0f;                   // The speed of the crawler
    [Export]
    private float TurnSpeed = 15f;                  // How fast the crawler turns in the corners
    [Export(PropertyHint.Enum,"Right, Left")]       // Create a drop-down for choosing direction
    private int CrawlDirection = 0;                 // The direction to crawl in. 0 = right, 1 = left
    private Vector2 SnapVector = Vector2.Zero;      // The vector to snap the crawler to the wall with
    private Vector2 Velocity = Vector2.Right;       // Initial direction is right, but will be overridden by the CrawlDirection
    private float TargetRotationAngle;              // The target rotation angle to rotate towards

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        TargetRotationAngle = Velocity.Angle();     // Initialize the target rotation angle to the same as the velocity angle
        InitCrawlerDirection();                     // Initialize the crawler direction, based on what is selected in the drop-down
    }
    private void InitCrawlerDirection()
    {
        // If the crawler direction is set to Right
        if(CrawlDirection == 0)
        {
            Velocity = Vector2.Right;               // Set velocity vector to: Right
        }
        // If the crawler direction is set to Left
        else if(CrawlDirection == 1)
        {
            Velocity = Vector2.Left;                // Set velocity vector to: Right
        }
    }

    private void SetVelocitySnapAndRotation(KinematicCollision2D collision)
    {
        // If the crawler is moving to the right
        if(CrawlDirection == 0)
        {
            Velocity = collision.Normal.Rotated(Mathf.Pi/2);  // Set velocity forward direction to point along the direction of the wall
            TargetRotationAngle = Velocity.Angle();           // Set target rotation to the Velocity angle
        }
        // If the crawler is moving to the left
        else if(CrawlDirection == 1)
        {
            Velocity = -collision.Normal.Rotated(Mathf.Pi/2); // Set velocity forward direction to point along the direction of the wall
            TargetRotationAngle = Velocity.Angle() + Mathf.Pi;// Set target rotation to the Velocity angle + rotate sprite 180 degrees
        }
        SnapVector = collision.Normal.Rotated((Mathf.Pi));    // Get the Snap vector - The direction pointing down towards the wall
    }

    private void RotateTowardsTargetVector(float delta)
    {
        // Linearly interpolate the rotation angle towards the target rotation angle, using the LerpAngle() method
        Rotation = Mathf.LerpAngle(Rotation, TargetRotationAngle, TurnSpeed * delta);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        var collision = MoveAndCollide(Velocity * Speed * delta);   // Move the wall crawler
        RotateTowardsTargetVector(delta);                           // Rotate the crawler towards the target vector

        // If the crawler is colliding with a wall
        if(collision != null)
        {
            SetVelocitySnapAndRotation(collision);                  // Se the velocity, snap vector and rotation angle for the crawler
        }
        // If the crawler isn't colliding with anything
        else if(collision == null)
        {
            // If the crawler isn't attached to the wall
            if(!IsOnFloor())
            {
                Velocity += SnapVector.Normalized();                // pull it towards the wall
            }
        }
    }
}
