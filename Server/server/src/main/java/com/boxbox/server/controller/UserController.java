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

    @PostMapping("/signup")
    public ApiResponse<String> signup(@RequestBody SignupRequest request) {
        try {
            userService.signup(request);
            return ApiResponse.success("signup success", null);
        } catch (IllegalArgumentException e) {
            return ApiResponse.fail(e.getMessage());
        }
    }

    @PostMapping("/login")
    public ApiResponse<User> login(@RequestBody LoginRequest request) {
        try {
            User user = userService.login(request);
            return ApiResponse.success("login success", user);
        } catch (IllegalArgumentException e) {
            return ApiResponse.fail(e.getMessage());
        }
    }

    @PostMapping("/savename")
    public ApiResponse<String> saveName(@RequestBody NameSave request) {
        try {
            userService.saveCharacterName(request);
            return ApiResponse.success("name save success", null);
        } catch (IllegalArgumentException e) {
            return ApiResponse.fail(e.getMessage());
        }
    }

    @GetMapping("/loadname")
    public ApiResponse<NameSave> loadName(
            @RequestParam int userId, 
            @RequestParam int index) {
        try {
            NameSave data = userService.loadCharacterName(userId, index); 
            return ApiResponse.success("name load success", data);
        } catch (IllegalArgumentException e) {
            return ApiResponse.fail(e.getMessage());
        }
    }
}