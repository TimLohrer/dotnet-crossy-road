export interface Skin {
    id: number;
    modelName: string;
    name: string;
    price: number;
    rarity: SkinRarity;
}

export enum SkinRarity {
    Common = 0,
    Uncommon = 1,
    Rare = 2,
    Legendary = 3
}