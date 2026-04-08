package com.boxbox.server.entity;

import jakarta.persistence.*;
import lombok.Getter;
import lombok.Setter;
import lombok.NoArgsConstructor;
import org.hibernate.annotations.CreationTimestamp; // 생성 시간 자동 기록용

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

    // 외래키 설정. User 테이블의 id와 연결됨
    @ManyToOne
    @JoinColumn(name = "user_id")
    private User user; 

    private int stageId;
    private int points;

    @CreationTimestamp // 데이터가 쌓일 때 현재 시간이 자동으로 들어감
    private LocalDateTime achievedAt;
}
