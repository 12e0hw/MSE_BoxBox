package com.boxbox.server.dto;

import lombok.Getter;
import lombok.Setter;

@Getter
@Setter
public class NameSave {
    // Character name slot data for save and load requests.
    private int userId;
    private int index;
    private String characterName;
}
