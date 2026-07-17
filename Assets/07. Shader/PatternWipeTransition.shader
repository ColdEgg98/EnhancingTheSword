Shader "UI/PatternWipeTransition"
{
    Properties
    {
        _PatternTex ("Pattern Texture", 2D) = "white" {}
        _TileCount ("Tile Count (X, Y)", Vector) = (8, 8, 0, 0)
        _Progress ("Progress", Range(0, 1)) = 0
        _EdgeSoftness ("Edge Softness", Range(0.001, 0.5)) = 0.08
        [Toggle] _ReverseDirection ("Reverse Direction (uncheck: TR->BL)", Float) = 0
        
        // [수정됨] unidirectional 트랜지션을 위한 모드 선택 프로퍼티 추가
        // [Enum(In_Cover,0, Out_Uncover,1)] _WipeMode ("Wipe Mode", Float) = 0
        _WipeMode ("Wipe Mode (0:In, 1:Out)", Float) = 0

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
            
            // [수정됨] 추가된 프로퍼티 선언
            float _WipeMode;

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
                // 1) 대각선 웨이브 마스크 계산
                // TR (0) -> BL (1)
                float dist = ((1.0 - i.texcoord.x) + (1.0 - i.texcoord.y)) * 0.5;

                if (_ReverseDirection > 0.5)
                {
                    dist = 1.0 - dist;
                }

                // [수정됨] unidirectional 트랜지션 및 잔상 방지 통합 로직
                // modeVal < 0.5 이면 In (0), modeVal > 0.5 이면 Out (1)
                float modeVal = _WipeMode > 0.5 ? 1.0 : 0.0;
                
                // 트랜지션 진행 방향 고정: Out일 때는 Progress 1->0을 0->1로 반전하여 사용
                float fixedProg = lerp(_Progress, 1.0 - _Progress, modeVal);
                
                // 진행 거리 확장 맵핑 (잔상 방지)
                float adjustedProgress = lerp(-_EdgeSoftness, 1.0 + _EdgeSoftness, fixedProg);
                
                // smoothstep 범위 설정
                float distMin = adjustedProgress - _EdgeSoftness;
                float distMax = adjustedProgress + _EdgeSoftness;

                // 기본 smoothstep 계산 (dist < E 이면 0, dist > E 이면 1 반환)
                float smoothVal = smoothstep(distMin, distMax, dist);
                
                // 모드에 따라 마스크 뒤집기: In일 때는 1-smoothVal (덮기), Out일 때는 smoothVal (걷기)
                float mask = lerp(1.0 - smoothVal, smoothVal, modeVal);
                
                // 2) 타일링 패턴 샘플링
                float2 tiledUV = i.texcoord * _TileCount.xy;
                fixed4 patternColor = tex2D(_PatternTex, tiledUV);

                // 3) 최종 색상 병합
                fixed4 col = patternColor * i.color;
                col.a *= mask;

                return col;
            }
            ENDCG
        }
    }
}