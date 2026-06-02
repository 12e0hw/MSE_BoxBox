package com.boxbox.server.dto;
import lombok.Getter;
import lombok.Setter;

@Getter
@Setter
public class NameSave {
    private int userId;
    private int index;
    private String characterName;
}
