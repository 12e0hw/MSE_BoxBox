package com.boxbox.server.controller;

import com.boxbox.server.entity.User;
import com.boxbox.server.service.UserService;
import com.boxbox.server.dto.*;
import com.boxbox.server.global.ApiResponse;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/users")
@RequiredArgsConstructor
@CrossOrigin(origins = "*") 
public class UserController {

    private final UserService userService;

    // Register a new user account.
    @PostMapping("/signup")
    public ApiResponse<String> signup(@RequestBody SignupRequest request) {
        try {
            userService.signup(request);
            return ApiResponse.success("Signup succeeded.", null);
        } catch (IllegalArgumentException e) {
            return ApiResponse.fail(e.getMessage());
        }
    }

    // Authenticate a user and return account data.
    @PostMapping("/login")
    public ApiResponse<User> login(@RequestBody LoginRequest request) {
        try {
            User user = userService.login(request);
            return ApiResponse.success("Login succeeded.", user);
        } catch (IllegalArgumentException e) {
            return ApiResponse.fail(e.getMessage());
        }
    }

    // Save one character name slot for a user.
    @PostMapping("/savename")
    public ApiResponse<String> saveName(@RequestBody NameSave request) {
        try {
            userService.saveCharacterName(request);
            return ApiResponse.success("Name saved successfully.", null);
        } catch (IllegalArgumentException e) {
            return ApiResponse.fail(e.getMessage());
        }
    }

    // Load one character name slot for a user.
    @GetMapping("/loadname")
    public ApiResponse<NameSave> loadName(
            @RequestParam int userId, 
            @RequestParam int index) {
        try {
            NameSave data = userService.loadCharacterName(userId, index); 
            return ApiResponse.success("Name loaded successfully.", data);
        } catch (IllegalArgumentException e) {
            return ApiResponse.fail(e.getMessage());
        }
    }
}
