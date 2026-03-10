# region CARD
public enum E_idCard {
    Nhà_đực,
    Nỏ_thần,
    Tường_măng_tre,
    Tháp_tằm,
    Tháp_giao_long,
    Lò_nung_gốm,
    Thánh_Gióng_nhỏ,
    Rùa_thần_pháo,
    Mộc_tinh,
    Đàn_chim_Lạc,
    Chum_ma_xó,
    Ngựa_sắt_9_hồng_mao,
    Bão_tên_đồng,
    Thủy_triều,
    Hỏa_cầu,
    Đất_mẹ_bảo_vệ,
    Sinh_lực_mẫu_thần,
    Ultimate,
    Dung_hợp,
    Thẻ_nâng_cấp,
}

public enum E_typeCard {
    Structure, Unit, Spell, Operation,
}

public enum E_targetCard {
    Ground, Air, Water,
}
#endregion

#region ENEMY
public enum E_idEnemy {
    Chuột_tinh,
    Quận_cú,
    Chằn_tinh,
    Tinh_bò_rừng,
    Ông_ba_bị,
    Bọ_cạp_tinh,
    Quạ_tinh,
    Rết_tinh,
    Dế_chũi,
    Chó_đội_nón_mê,
    Tinh_đỉa,
    Cá_sấu_5_chân,
    Cá_bống_tinh,
    Ma_da,
    Ngư_tinh,
}

public enum E_areaEnemy {
    Đồng_cỏ, Vùng_nóng, Vùng_lạnh,
}

public enum E_typeEnemy {
    Normal, MiniBoss, Boss,
}

public enum E_typeMoveEnemy {
    Đi_bộ, Bay, Lòng_đất, Bơi
}

public enum E_typeTaget_EnemyNotAtk {
    None, Structure,
}
#endregion

[System.Serializable]
public struct S_baseStats {
    public E_idBaseStats id;
    public float value;
}

public enum E_idBaseStats {
    Hp, Atk, SpeedMove, SpeedAtk, RangeAtk
}

public enum E_element {
    Kim, Mộc, Thủy, Hỏa, Thổ, None
}