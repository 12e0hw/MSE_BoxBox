// User Information Entities

package com.boxbox.server.entity;
import jakarta.persistence.*;
import lombok.AccessLevel;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Entity
@Getter 
@Setter
//JPA가 엔티티를 생성할 수 있게 기본 생성자를 만들고 protected로 빈 객체 생성을 제한함
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Table(name = "users") 
public class User {
    
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY) // 번호 자동 증가
    @Column(name = "user_id")
    private Long userId;

    @Column(name = "login_id", nullable = false, unique = true, length = 50)
    private String loginId;

    @Column(name = "password", nullable = false, length = 255)
    private String password;

    @Column(name = "username", nullable = false, length = 50)
    private String username;

    public User(String loginId, String password, String username){
        this.loginId = loginId;
        this.password = password;
        this.username = username;
    }
    
}