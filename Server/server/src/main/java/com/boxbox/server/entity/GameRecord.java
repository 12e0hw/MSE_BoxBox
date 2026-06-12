package com.boxbox.server.entity;

import jakarta.persistence.*;
import lombok.Getter;
import lombok.Setter;
import lombok.NoArgsConstructor;
import org.hibernate.annotations.CreationTimestamp;

import java.time.LocalDateTime;

@Entity
@Getter 
@Setter
@NoArgsConstructor
@Table(name = "game_records")
public class GameRecord {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long recordId;

    // Link each score record to the user who achieved it.
    @ManyToOne
    @JoinColumn(name = "user_id")
    private User user; 

    @Column(name = "stage_id", nullable = false)
    private int stageId;

    @Column(name = "points", nullable = false)
    private int points;

    // Automatically stores the creation time when the record is inserted.
    @CreationTimestamp
    @Column(name = "achieved_at")
    private LocalDateTime achievedAt;
}
