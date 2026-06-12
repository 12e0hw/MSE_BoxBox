package com.boxbox.server.entity;

import jakarta.persistence.*;
import lombok.*;

@Entity
@Getter 
@Setter
@NoArgsConstructor
@Table(name = "users") 
public class User {
    
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY) // Auto-increment user id.
    @Column(name = "user_id")
    private Long userId;

    // @Column(name = "login_id", nullable = false, unique = true, length = 50)
    // private String loginId;

    @Column(name = "password", nullable = false, length = 255)
    private String password;

    @Column(name = "username", nullable = false, length = 50)
    private String username;

    @Column(name = "character_name_1")
    private String characterName1;

    @Column(name = "character_name_2")
    private String characterName2;

    public User(String password, String username){
        this.password = password;
        this.username = username;
    }

    public void updateCharacterName1(String name) {
        this.characterName1 = name;
    }

    public void updateCharacterName2(String name) {
        this.characterName2 = name;
    }
    
}
