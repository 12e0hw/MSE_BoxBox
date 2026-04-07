package com.boxbox.server.repository;

import com.boxbox.server.entity.User; 
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;
import java.util.Optional;

@Repository
public interface UserRepository extends JpaRepository<User, Long> {
    
    // 로그인할 때 아이디로 사용자를 찾기 위한 메서드
    // Optional을 쓰면 사용자가 없을 경우의 예외 처리가 쉬워집니다.
    Optional<User> findByLoginId(String loginId);
}