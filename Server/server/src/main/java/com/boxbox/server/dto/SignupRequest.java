package com.boxbox.server.dto;
import lombok.Getter;

@Getter
public class SignupRequest {
    private String loginId;
    private String password;
    private String username;
}