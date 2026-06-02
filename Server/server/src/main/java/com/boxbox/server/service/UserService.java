package com.boxbox.server.service;

import com.boxbox.server.entity.User;
import com.boxbox.server.repository.UserRepository;
import com.boxbox.server.dto.*;

import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

@Service
@RequiredArgsConstructor
public class UserService {

    private final UserRepository userRepository;

    // Signup
    public void signup(SignupRequest dto) {
        if (userRepository.existsByUsername(dto.getUsername())) {
            throw new IllegalArgumentException("Duplicate Username");
        }

        User user = new User(dto.getPassword(), dto.getUsername());
        userRepository.save(user);
    }

    // Login
    public User login(LoginRequest dto) {
        User user = userRepository.findByUsername(dto.getUsername())
                .orElseThrow(() -> new IllegalArgumentException("Username does not exist"));

        if (!user.getPassword().equals(dto.getPassword())) {
            throw new IllegalArgumentException("Password Mismatch");
        }
        return user;
    }

    @Transactional
    public void saveCharacterName(NameSave request) {
        User user = userRepository.findById((long) request.getUserId())
            .orElseThrow(() -> new IllegalArgumentException("Empty."));

        if (request.getIndex() == 1) {
            user.updateCharacterName1(request.getCharacterName());
        } else if (request.getIndex() == 2) {
            user.updateCharacterName2(request.getCharacterName());
        } else {
            throw new IllegalArgumentException("index 1 or 2.");
        }
    }

    public NameSave loadCharacterName(int userId, int index) {
        User user = userRepository.findById((long) userId)
            .orElseThrow(() -> new IllegalArgumentException("Empty."));

        String loadedName = "";

        if (index == 1) {
            loadedName = user.getCharacterName1();
        } else if (index == 2) {
            loadedName = user.getCharacterName2();
        } else {
            throw new IllegalArgumentException("index 1 or 2.");
        }

        NameSave responseDto = new NameSave();
        responseDto.setUserId(userId);
        responseDto.setIndex(index);
        responseDto.setCharacterName(loadedName);

        return responseDto;
    }
}