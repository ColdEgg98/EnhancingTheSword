// PatternWipeTransition.shader
// 우상단(1,1) -> 좌하단(0,0) 방향으로 대각선 웨이브가 번지며
// 타일링 패턴이 화면을 채우는(또는 걷히는) UI 전환 셰이더.
// Built-in RP 전용. Canvas 위 RawImage 등 UI 오브젝트에 붙여서 사용.
//
// 사용법 요약:
//  1) 이 셰이더로 머티리얼 생성
//  2) _PatternTex 에 타일링 패턴 PNG 지정 (Wrap Mode = Repeat 권장)
//  3) 화면을 덮는 RawImage 에 이 머티리얼을 연결
//  4) C#에서 material.SetFloat("_Progress", 0~1 사이 값) 을 DOTween으로 애니메이션
//     - _Progress = 0 : 완전히 안 보임 (아직 안 덮임)
//     - _Progress = 1 : 완전히 다 덮임 (화면 전체가 패턴으로 채워짐)

Shader "UI/PatternWipeTransition"
{
    Properties
    {
        // 반복(tiling)해서 그릴 작은 패턴 이미지
        _PatternTex ("Pattern Texture", 2D) = "white" {}

        // 패턴이 화면에 몇 번 반복될지 (X, Y 각각 몇 칸)
        _TileCount ("Tile Count (X, Y)", Vector) = (8, 8, 0, 0)

        // 0 = 하나도 안 덮임, 1 = 전체가 덮임. DOTween 으로 이 값을 애니메이션.
        _Progress ("Progress", Range(0, 1)) = 0

        // 웨이브 경계가 딱딱하게 잘리지 않고 부드럽게 퍼지는 정도 (0에 가까울수록 각짐, 클수록 부드러움)
        _EdgeSoftness ("Edge Softness", Range(0.001, 0.5)) = 0.08

        // 대각선 웨이브 방향을 뒤집고 싶을 때 사용 (1 = 우상단->좌하단, -1 = 반대)
        [Toggle] _ReverseDirection ("Reverse Direction (uncheck: TR->BL)", Float) = 0

        // UI 표준 프로퍼티 (마스킹 등 호환용, 건드릴 필요 없음)
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _PatternTex;
            float4 _PatternTex_ST;
            float4 _TileCount;
            float _Progress;
            float _EdgeSoftness;
            float _ReverseDirection;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // --- 1) 대각선 웨이브 마스크 계산 ---
                // uv (0,0)=좌하단, (1,1)=우상단 기준.
                // "우상단에서 시작해 좌하단으로 번진다" = 우상단일수록 먼저(작은 값) 덮여야 함.
                // 대각선 거리는 (1-u)+(1-v) 를 0~1로 정규화해서 사용.
                // u=1,v=1(우상단) 일 때 dist=0 (제일 먼저 채워짐)
                // u=0,v=0(좌하단) 일 때 dist=1 (제일 나중에 채워짐)
                float dist = ((1.0 - i.texcoord.x) + (1.0 - i.texcoord.y)) * 0.5;

                if (_ReverseDirection > 0.5)
                {
                    dist = 1.0 - dist;
                }

                // smoothstep 으로 진행도(_Progress) 기준 부드러운 경계 마스크 생성
                // dist < _Progress - softness 이면 완전히 보임(1)
                // dist > _Progress + softness 이면 완전히 안 보임(0)
                float mask = 1.0 - smoothstep(_Progress - _EdgeSoftness, _Progress + _EdgeSoftness, dist);

                // --- 2) 타일링 패턴 샘플링 ---
                float2 tiledUV = i.texcoord * _TileCount.xy;
                fixed4 patternColor = tex2D(_PatternTex, tiledUV);

                // --- 3) 최종 색상: 패턴 알파 * 웨이브 마스크 * UI Graphic 색상(틴트) ---
                fixed4 col = patternColor * i.color;
                col.a *= mask;

                return col;
            }
            ENDCG
        }
    }
}
