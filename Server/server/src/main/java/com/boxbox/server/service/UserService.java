package com.boxbox.server.service;

import com.boxbox.server.entity.User;
import com.boxbox.server.repository.UserRepository;
import com.boxbox.server.dto.*;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

@Service
@RequiredArgsConstructor
public class UserService {

    private final UserRepository userRepository;

    // Signup
    public void signup(SignupRequest dto) {
        if (userRepository.findByLoginId(dto.getLoginId()).isPresent()) {
            throw new IllegalArgumentException("Duplicate ID");
        }

        User user = new User();
        user.setLoginId(dto.getLoginId());
        user.setPassword(dto.getPassword());  
        user.setUsername(dto.getUsername());

        userRepository.save(user);
    }

    // Login
    public User login(LoginRequest dto) {
        User user = userRepository.findByLoginId(dto.getLoginId())
                .orElseThrow(() -> new IllegalArgumentException("ID does not exist"));

        if (!user.getPassword().equals(dto.getPassword())) {
            throw new IllegalArgumentException("Password Mismatch");
        }

        return user;
    }
}