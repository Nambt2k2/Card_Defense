# region CARD
public enum E_idCard {
    Nhà_đực = 0,
    Nỏ_thần = 1,
    Tường_măng_tre = 2,
    Tháp_tằm = 3,
    Tháp_giao_long = 4,
    Lò_nung_gốm = 5,
    Thánh_Gióng_nhỏ = 6,
    Rùa_thần_pháo = 7,
    Mộc_tinh = 8,
    Đàn_chim_Lạc = 9,
    Chum_ma_xó = 10,
    Ngựa_sắt_9_hồng_mao = 11,
    Bão_tên_đồng = 12,
    Thủy_triều = 13,
    Hỏa_cầu = 14,
    Đất_mẹ_bảo_vệ = 15,
    Sinh_lực_mẫu_thần = 16,
    Ultimate = 17,
    Dung_hợp = 18,
    Thẻ_nâng_cấp = 19,
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
    Chuột_tinh = 0,
    Quận_cú = 1,
    Chằn_tinh = 2,
    Tinh_bò_rừng = 3,
    Ông_ba_bị = 4,
    Bọ_cạp_tinh = 5,
    Quạ_tinh = 6,
    Rết_tinh = 7,
    Dế_chũi = 8,
    Chó_đội_nón_mê = 9,
    Tinh_đỉa = 10,
    Cá_sấu_5_chân = 11,
    Cá_bống_tinh = 12,
    Ma_da = 13,
    Ngư_tinh = 14,
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

public enum E_stateGame {
    Init, Playing, Lose, Win
}

public enum E_idBaseStats {
    Hp, Atk, SpeedMove, SpeedAtk, RangeAtk
}

public enum E_element {
    Kim, Mộc, Thủy, Hỏa, Thổ, None
}